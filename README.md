# Working with enums and Ef core

We want to manage in an easy way enum values in our entities fullfilling following requirement:

- Entity getter and setter use enum type, not int, so any interaction with entity is done using enums. Example:

        var entity=new Student { Name=Bob", Rating = Rating.Brilliant }

- A catalogue table is created in database for each enum type
- Catalogue table is automatically seeded with enum values
- A foreign key is created from entity to catalogue table
- Foreign key is automatically managed, using an enum field

## The sample

We have an entity (Student) with following fields:

|Field   | Type  |   |
|---|---|---|
| Id  | int  |   |
| Name | string  |   |
| Rating | Rating (enum)  |   |

Rating possible values: Brilliant/Good/Average/Bad/Terrible

## Attempt1: Catalogue table rows are managed automatically

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

Define your entity, an entity for Catalogue table

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

Finally we use helper to populate values

```c#
    using (var context = GetDbContext())
    {
        context.Database.EnsureCreated();
        Seeder.SeedEnumData<Rating, RatingEnum>(context.Ratings);
        context.SaveChanges();
    }
```

Now we can use it

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

**Problems we found in this approach**: we have to cast as the property in hgt eentrity is int not an enum

## Attemp 2: we use backing fields

We use backing fields to instruct EF core to manage a field instead of a property (https://docs.microsoft.com/en-us/ef/core/modeling/backing-field).

In additon we expose a property that perform de casting between int and enum

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
    ...
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Property(b => b.RatingId)
                .HasField("_ratingId");
        }
    }
```

Now we can use it without casting:

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

Unfortunatelly this is not working due some Ef Core limitations, thar require field and property to be of assignable types:

*Unhandled Exception: System.InvalidOperationException: The specified field '_ratingId' of type 'int' cannot be used for the property 'Student.RatingId' of type 'RatingEnum'. Only backing fields of types that are assignable from the property type can be used.*

## Attempt 3

In order to fix the error found in previous attempt, we have to:

- Instruct Ef core to ignore the property based on enum () as we are going to manage it
- Define a backing field that is not connected to a property
- Configure foreign key to use backing field

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

With this approach everything is working fine, we can use enums and everything works as expected

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