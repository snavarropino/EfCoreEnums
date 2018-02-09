# Working with enums and Ef core

We want to manage in an easy way enum values in our entities fullfilling following requirements:

- Entity getter and setter must use enum type, not int, so any interaction with entity is done using enums. Example:

        var entity=new Student { Name="Bob", Rating = Rating.Brilliant }

- A catalogue table is automatically created in database for each enum type
- Catalogue table is automatically seeded with enum values
- A foreign key is created from entity to catalogue table
- Foreign key is automatically managed, using an enum field

## The sample

We have an entity (Student) with following fields:

|Field   | Type  |
|---|---|
| Id  | int  |
| Name | string  |
| Rating | Rating (enum)  |

Rating possible values: Brilliant/Good/Average/Bad/Terrible

## Attempt 1: Catalogue table rows are managed automatically

We create some infrastructure that help us in the creation of the catalogue table. In addition values are automatically populated.

Solution is based in:

- A base class, that helps us in the creation of the catalogue table: EnumBase
- A helper class that help us to seed data

Steps:

Define the enum

```c#
    public enum RatingEnum
    {
        [Description("Something really good")]
        Brilliant = 1,
        Good = 2,
        Average = 3,
        Bad = 4,
        [Description("Something really bad")]
        Terrible = 5,
    }
```

Define your entity (student), and defina also an entity for the catalogue table (Rating)

```c#
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RatingId { get; set; }
        public Rating Rating { get; set; }
    }

    public class Rating: EnumBase<RatingEnum>
    {
    }
```

Finally we use helper seed method to populate values

```c#
    using (var context = GetDbContext())
    {
        context.Database.EnsureCreated();
        Seeder.SeedEnumData<Rating, RatingEnum>(context.Ratings);
        context.SaveChanges();
    }
```

Now we can use student entity:

```c#
    private static void AddStudentPepe()
    {
        using (var context = GetDbContext())
        {
            var pepe = new Student()
            {
                Name = "Pepe",
                RatingId = (int)RatingEnum.Bad
            };

            context.Students.Add(pepe);
            context.SaveChanges();
        }
    }

    private static void UpdateStudentPepe()
    {
        using (var context = GetDbContext())
        {
            var pepe = context.Students.First(a=>a.Name=="pepe");
            pepe.RatingId = (int)RatingEnum.Brilliant;
            context.SaveChanges();
        }
    }
```

**Problems we found in this approach**: we have to cast as the property because it is not an enum in our entity

## Attemp 2: we use backing fields

We use backing fields to instruct EF core to manage a field instead of a property (https://docs.microsoft.com/en-us/ef/core/modeling/backing-field).

In additon we expose a property that performst the casting between int and enum

```c#
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public RatingEnum RatingId
        {
            get =>  (RatingEnum) _ratingId;
            set => _ratingId = (int) value;
        }

        private int _ratingId;

        [ForeignKey("_ratingId")]
        public Rating Rating { get; set; }
    }

    public class StudentDbContext: DbContext
    {
        [...]

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Property(b => b.RatingId)
                .HasField("_ratingId");
        }
    }
```

Now we can use our student entity without any casting:

```c#
    using (var context = GetDbContext())
    {
        var pepe = new Student()
        {
            Name = "Pepe",
            RatingId = RatingEnum.Bad
        };

        context.Students.Add(pepe);
        context.SaveChanges();
        Log.Information("Student added: {@student}", pepe);
    }
```

Unfortunatelly this is not working due some Ef Core limitations: Entity framework core requires that field and property have to be of assignable types:

*Unhandled Exception: System.InvalidOperationException: The specified field '_ratingId' of type 'int' cannot be used for the property 'Student.RatingId' of type 'RatingEnum'. Only backing fields of types that are assignable from the property type can be used.*

## Attempt 3

In order to fix the previous error we have to:

- Instruct Ef core to ignore the property based on enum (RatingId) as we are going to manage it
- Define a backing field that is not connected to a property
- Configure foreign key to use the defined backing field

```c#
    public class StudentDbContext: DbContext
    {
        ...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Ignore(s => s.RatingId) //Ignore enum
                .Property<int>("_ratingId"); //Define backing field with no property

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Rating)
                .WithMany()
                .HasForeignKey("_ratingId"); //Set FK to backing field
        }
    }
```

Our entity is as showed below (similar to previous attempt but removing FK annotation)

```c#
    public class Student
    {

        public int Id { get; set; }
        public string Name { get; set; }


        public RatingEnum RatingId
        {
            get => (RatingEnum)_ratingId;
            set => _ratingId = (int)value;
        }

        private int _ratingId;

        public Rating Rating { get; set; }
    }
```

With this approach everything is working fine: **we can use enums** and everything works as expected

```c#
    private static void AddStudentPepe()
    {
        using (var context = GetDbContext())
        {
            var pepe = new Student()
            {
                Name = "Pepe",
                RatingId = RatingEnum.Bad
            };

            context.Students.Add(pepe);
            context.SaveChanges();
        }
    }

    private static void UpdateStudentPepe()
    {
        using (var context = GetDbContext())
        {
            var pepe = context.Students.First();
            pepe.RatingId = RatingEnum.Brilliant;
            context.SaveChanges();
        }
    }
```
However there are still several improvements we can do. In this case, if we look to the created database diagram, we notice thar the foreign key field naming is not so good:

![Database diagram](https://github.com/snavarropino/EfCoreEnums/blob/master/images/DiagramAtempt3.png?raw=true "Database diagram")

In adition, the property we created in our student class has a not very good name: RatingId, it would be great to name it Rating. Let's do it!

## Attempt 3 Improved

In order to improve the point we noticed, let's modify student entity:

We also have to modify our DbContext to instruct Entity Framerork core in the right way:

```c#
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public RatingEnum Rating //Improved Name
        {
            get => (RatingEnum)_ratingId;
            set => _ratingId = (int)value;
        }

        private int _ratingId;
        public Rating RatingCatalogue { get; set; }
    }
```


```c#
    public class StudentDbContext: DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options): base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Ignore(s => s.Rating)      // Ignore enum property
                .Property<int>("_ratingId") // Define backing field with no property
                .HasColumnName("RatingId")  // Set proper column name for foreign key
                .IsRequired();

            modelBuilder.Entity<Student>()
                .HasOne(s => s.RatingCatalogue)
                .WithMany()
                .HasForeignKey("_ratingId") // Set foreign key to backing field
                .IsRequired();
        }
    }
```

With previous changes, our entity naming is much better. In addition, our databse model looks great!

![Database diagram](https://github.com/snavarropino/EfCoreEnums/blob/master/images/DiagramAtempt3_Improved.png?raw=true "Database diagram")


Hope this helps!

Sergio



