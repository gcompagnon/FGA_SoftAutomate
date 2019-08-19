using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FGABusinessComponent.BusinessComponent.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exemple
{
//    [Table("PERSON", Schema = "ex")]
    public class Person
    {
//        [Key]
        public int PersonId { get; set; }
//        [Required]
        public string Name { get; set; }
        public bool Sex { get; set; }
//        [MaxLength(4)]
        public char[] Title { get; set; }
        public string Address1 { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }



        public int? BagId { get; set; }
        public Bag Bag { get; set; }
    }

//    [Table("AUTHOR", Schema = "ex")]
    public class Author : Person
    {
        public Author()
        {
//            Books = new List<Book>();
        }
        //[Key]
        //public int AuthorId { get; set; }
//        public DateTime DOB { get; set; }
        // Author have collect of Books
        public string EditorHouse { get; set; }
        public virtual ICollection<Book> Books { get; set; }


    }

    public class Bag
    {
        public int BagId { get; set; }
        public string Contenu { get; set; }
        
        public int OwnerId { get; set; }
        public Person Owner { get; set; }
    }


//    [Table("BOOK",Schema = "ex")]
    public class Book
    {
//        [Key]
        public int BookId { get; set; }
//        [Required]
        public string Name { get; set; }
        public DateTime PublishDate { get; set; }
        // Ref. for the Author . It helps to relate the two class
        // and also helps to create the EF Code First data Base
//        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public virtual Author Author { get; set; } //virtual property to access Author LAZY LOADING

        // gestion de la concurrence
        public byte[] RowVersion { get; set; }
    }

    /// <summary>
    /// le gestionnaire entity framework 
    /// </summary>
    public class ProductContext : DbContext
    {

        public ProductContext(String databaseName)
            : base(databaseName)
        { Database.SetInitializer(new ProductContextInitializer()); }

        public ProductContext()
            : base("name=DEV")
        {

            Database.SetInitializer(new ProductContextInitializer());
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BookDataModelConfiguration());
            modelBuilder.Configurations.Add(new AuthorDataModelConfiguration());
            modelBuilder.Configurations.Add(new PersonDataModelConfiguration());
            modelBuilder.Configurations.Add(new BagDataModelConfiguration());
            // nommer les tables
            //modelBuilder.Entity<Book>().ToTable("ex.BOOK");
            //modelBuilder.Entity<Author>().ToTable("ex.AUTHOR");
            // supprimer la table EdmMetaData
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
        } 

    }

    /// <summary>
    /// configuration Fluent API pour Book
    /// </summary>
    public class BookDataModelConfiguration : EntityTypeConfiguration<Book>
    {
        public BookDataModelConfiguration()
        {
            this.ToTable(tableName: "BOOK", schemaName: "ex");
            this.HasKey(d => d.BookId);
            // faire en sorte que la clé soit généré par la base
            Property(d => d.BookId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(d => d.Name).HasMaxLength(500);
            Property(d => d.Name).IsRequired();

            // la foreign key : relation Author 1->* Book
            this.HasRequired(s => s.Author).WithMany(l => l.Books).HasForeignKey(s => s.AuthorId);

            // specifier une colonne timestamp pour gerer la concurrence => OptimisticConcurrencyException
            Property(d => d.RowVersion).IsRowVersion();
        }
    }
    public class BagDataModelConfiguration : EntityTypeConfiguration<Bag>
    {
        public BagDataModelConfiguration()
        {
            this.ToTable(tableName: "BAG", schemaName: "ex");
            this.HasKey(d => d.BagId);
            Property(d => d.Contenu).IsRequired();

            // astuce pour avoir une relation one-to-one bi directionnel
            this.HasRequired(a => a.Owner).WithMany().HasForeignKey(b => b.OwnerId);
            
        }
    }

    /// <summary>
    /// configuration Fluent API pour Book
    /// </summary>
    public class AuthorDataModelConfiguration : EntityTypeConfiguration<Author>
    {
        public AuthorDataModelConfiguration()
        {
            this.ToTable(tableName: "AUTHOR", schemaName: "ex");
            //this.HasKey(d => d.PersonId);
            //Property(d => d.Name).IsRequired();

           
            
        }
    }

    public class PersonDataModelConfiguration : EntityTypeConfiguration<Person>
    {
        public PersonDataModelConfiguration()
        {
            this.ToTable(tableName: "PERSON", schemaName: "ex");
            this.HasKey(d => d.PersonId);
            Property(d => d.Name).IsRequired();

            // astuce pour avoir une relation one-to-one bi directionnel
            this.HasOptional(b => b.Bag).WithMany().HasForeignKey(p => p.BagId);
        }
    }

    public class ProductContextInitializer :
                DropCreateDatabaseAlways<ProductContext>
//             DropCreateDatabaseIfModelChanges<ProductContext>
    {

        protected override void Seed(ProductContext context)
        {
            base.Seed(context);

            // creation des contraintes pour la relation one-to-one 
            context.Database.ExecuteSqlCommand("ALTER TABLE ex.BAG ADD CONSTRAINT UC_Bag_Owner UNIQUE (OwnerId)");
            //CREATE UNIQUE INDEX IX_Bag_Owner ON ex.BAG(OwnerId)

            context.Database.ExecuteSqlCommand("ALTER TABLE ex.PERSON ADD CONSTRAINT UC_Person_bag UNIQUE (BagId)");
            //CREATE UNIQUE INDEX IX_Author_Bag ON ex.PERSON(BagId)
        }
    }

    [TestClass()]
    public class ExempleEF 
    {

        [TestMethod()]
        public void EFLibraryExemple()
        {


            using (var db = new ProductContext())
            {
#if DEBUG
                EFCodeFirstMethods.DumpDbCreationScriptToFile(db);
#endif
                 
                // Add a food category 
//                var food = new Book { BookId = 1, Name = "Foods", AuthorId = 1, PublishDate = DateTime.Now };

                var bag = new Bag { BagId=1,Contenu ="TOOTO"};

                var fred = new Author { PersonId = 1, Name = "Fred Peters", EditorHouse="Gallimard", Sex= true };
                db.Authors.Add(fred);
                db.SaveChanges();

                fred.Bag = bag;
                bag.Owner = fred;
                db.SaveChanges();


                Book food = new Book { BookId = 1, Name = "Foods", PublishDate = DateTime.Now, Author = fred };
                db.Books.Add(food);
                Book roman = new Book { Name = "Germinal", PublishDate = DateTime.Now, Author = fred };
                db.Books.Add(roman);

                
                int recordsAffected = db.SaveChanges();
                Console.WriteLine("Saved {0} entities to the database, press any key to exit.", recordsAffected);                


                Author aut = db.Authors.FirstOrDefault();
                int i = 0;
                foreach(Book bk in aut.Books )
                {
                    i++;
                    Console.WriteLine("L auteur {0} a ecrit {1} ", aut.Name, bk.Name);
                }
                Assert.AreEqual(2,i);
                //Console.ReadKey();
                //db.Authors.Remove(aut);

                db.SaveChanges();
//                Console.ReadKey();
                


            }
        }
    }
}
