using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using pb;
using pb.Linq;

namespace Test.Test_CS.Test_Linq
{
    public static class Test_Linq_Join_f
    {
        public static void Test_Linq_InnerJoin_ProductCategory_LinqRequest_01()
        {
            // Inner Join using linq request : from ... in products join ... in categories on ... select ...
            Trace.WriteLine("Inner Join using linq request : from ... in products join ... in categories on ... select ...");
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            IEnumerable<ProductCategory> query =
                from product in products
                join category in categories on product.CategoryId equals category.Id
                select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category.Id, CategoryName = category.Name };
            Write(query);
        }

        public static void Test_Linq_InnerJoin_ProductCategory_LinqMethod_01()
        {
            // Inner Join using linq methoq : from ... in products join ... in categories on ... select ...
            Trace.WriteLine("Inner Join using linq methoq : from ... in products join ... in categories on ... select ...");
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            IEnumerable<ProductCategory> query =
                products.Join(
                    categories,
                    product => product.CategoryId,
                    category => category.Id,
                    (product, category) => new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category.Id, CategoryName = category.Name }
                    );
            Write(query);
        }

        public static void Test_Linq_InnerJoin_CategoryProduct_LinqRequest_01()
        {
            // Inner Join using linq request : from ... in categories join ... in products on ... select ...
            Trace.WriteLine("Inner Join using linq request : from ... in categories join ... in products on ... select ...");
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            // Inner Join (Jointure interne)
            IEnumerable<ProductCategory> query =
                from category in categories
                join product in products on category.Id equals product.CategoryId
                select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category.Id, CategoryName = category.Name }; //produces flat sequence
            Write(query);
        }

        public static void Test_Linq_InnerJoin_CategoryProduct_LinqMethod_01()
        {
            // Inner Join using linq methoq : from ... in categories join ... in products on ... select ...
            Trace.WriteLine("Inner Join using linq methoq : from ... in categories join ... in products on ... select ...");
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            IEnumerable<ProductCategory> query =
                categories.Join(
                    products,
                    category => category.Id,
                    product => product.CategoryId,
                    (category, product) => new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category.Id, CategoryName = category.Name }
                    );
            Write(query);
        }

        public static void Test_Linq_GroupJoin_ProductCategory_LinqRequest_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            // Left Outer Join
            IEnumerable<ProductCategories> query =
                from product in products
                join category in categories on product.CategoryId equals category.Id into categoryGroup
                select new ProductCategories { ProductId = product.Id, ProductName = product.Name, Categories = categoryGroup };

            Write(query);
        }

        public static void Test_Linq_GroupJoin_ProductCategory_LinqMethod_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            IEnumerable<ProductCategories> query =
                products.GroupJoin(
                    categories,                                        // IEnumerable<TInner> inner
                    product => product.CategoryId,                     // Func<TOuter, TKey> outerKeySelector
                    category => category.Id,                           // Func<TInner, TKey> innerKeySelector
                    (product, categoryGroup) =>                        // Func<TOuter, IEnumerable<TInner>, TResult> resultSelector
                        new ProductCategories { ProductId = product.Id, ProductName = product.Name, Categories = categoryGroup }
                    );

            Write(query);
        }

        public static void Test_Linq_GroupJoin_CategoryProduct_LinqRequest_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            // Group Join
            var query =
                from category in categories
                join product in products on category.Id equals product.CategoryId into productGroup
                select new CategoryProducts { CategoryId = category.Id, CategoryName = category.Name, Products = productGroup };

            Write(query);
        }

        public static void Test_Linq_LeftOuterJoin_ProductCategory_LinqRequest_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            // Left Outer Join
            IEnumerable<ProductCategory> query =
                from product in products
                join category in categories on product.CategoryId equals category.Id into categoryGroup
                from category2 in categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null })
                select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category2.Id, CategoryName = category2.Name };

            Write(query);
        }

        public static void Test_Linq_LeftOuterJoin_ProductCategory_LinqRequest_Expression_01()
        {
            Product[] products = null;
            Category[] categories = null;

            Expression<Func<IEnumerable<ProductCategory>>> lambdaQuery = () =>
                from product in products
                join category in categories on product.CategoryId equals category.Id into categoryGroup
                from category2 in categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null })
                select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category2.Id, CategoryName = category2.Name };

            Trace.WriteLine("{0}", lambdaQuery);

            // output :

            //() => value(Test.Test_CS.Test_Linq.Test_Linq_Join_f+<>c__DisplayClass45).products.GroupJoin(value(Test.Test_CS.Test_Linq.Test_Linq_Join_f+<>c__DisplayClass45).categories, product => product.CategoryId, category => Convert(category.Id), (product, categoryGroup) => new <>f__AnonymousType2`2(product = product, categoryGroup = categoryGroup)).SelectMany(<>h__TransparentIdentifier43 => <>h__TransparentIdentifier43.categoryGroup.DefaultIfEmpty(new Category() {Id = 0, Name = null}), (<>h__TransparentIdentifier43, category2) => new ProductCategory() {ProductId = <>h__TransparentIdentifier43.product.Id, ProductName = <>h__TransparentIdentifier43.product.Name, CategoryId = category2.Id, CategoryName = category2.Name})

            //products.GroupJoin(
            //    categories,
            //    product => product.CategoryId,
            //    category => Convert(category.Id),
            //    (product, categoryGroup) => new { product = product, categoryGroup = categoryGroup }
            //    ).SelectMany(
            //    <>h__TransparentIdentifier43 => <>h__TransparentIdentifier43.categoryGroup.DefaultIfEmpty(new Category() {Id = 0, Name = null}),
            //    (<>h__TransparentIdentifier43, category2) => new ProductCategory() {ProductId = <>h__TransparentIdentifier43.product.Id, ProductName = <>h__TransparentIdentifier43.product.Name, CategoryId = category2.Id, CategoryName = category2.Name}
            //    )
        }

        public static void Test_Linq_LeftOuterJoin_ProductCategory_LinqMethod_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();

            IEnumerable<ProductCategory> query =
                products.GroupJoin(
                    categories,                                        // IEnumerable<TInner> inner
                    product => product.CategoryId,                     // Func<TOuter, TKey> outerKeySelector
                    category => category.Id,                           // Func<TInner, TKey> innerKeySelector
                    (product, categoryGroup) =>                        // Func<TOuter, IEnumerable<TInner>, TResult> resultSelector
                        new { product, categoryGroup }
                    ).SelectMany(
                        // Func<TSource, IEnumerable<TCollection>> collectionSelector
                        source => source.categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null }),
                        // Func<TSource, TCollection, TResult> resultSelector
                        (source, category2) => new ProductCategory { ProductId = source.product.Id, ProductName = source.product.Name, CategoryId = category2.Id, CategoryName = category2.Name }
                    );
            Write(query);
        }

        public static void Test_Linq_LeftOuterJoin_ProductCategory_LinqRequest_02()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            // Left Outer Join
            var query =
                from product in products
                join category in categories on product.CategoryId equals category.Id into categoryGroup
                from category2 in categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null })
                where category2.Id == 0
                select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category2.Id, CategoryName = category2.Name };

            Write(query);
        }

        public static void Test_Linq_LeftOuterJoin_ProductCategory_LinqMethod_02()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();

            IEnumerable<ProductCategory> query =
                products.GroupJoin(
                    categories,                                        // IEnumerable<TInner> inner
                    product => product.CategoryId,                     // Func<TOuter, TKey> outerKeySelector
                    category => category.Id,                           // Func<TInner, TKey> innerKeySelector
                    (product, categoryGroup) =>                        // Func<TOuter, IEnumerable<TInner>, TResult> resultSelector
                        new { product, categoryGroup }
                    ).SelectMany(
                // Func<TSource, IEnumerable<TCollection>> collectionSelector
                        source => source.categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null }),
                // Func<TSource, TCollection, TResult> resultSelector
                        (source, category2) => new ProductCategory { ProductId = source.product.Id, ProductName = source.product.Name, CategoryId = category2.Id, CategoryName = category2.Name }
                    ).Where(source => source.CategoryId == 0);
            Write(query);
        }

        public static void Test_Linq_LeftOuterJoin_CategoryProduct_LinqRequest_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            // Left Outer Join
            var query =
                from category in categories
                join product in products on category.Id equals product.CategoryId into productGroup
                from product2 in productGroup.DefaultIfEmpty(new Product { Id = 0, Name = null, CategoryId = null })
                select new ProductCategory { ProductId = product2.Id, ProductName = product2.Name, CategoryId = category.Id, CategoryName = category.Name };

            Write(query);
        }

        public static void Test_Linq_LeftOuterJoin_CategoryProduct_LinqRequest_02()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            // Left Outer Join
            var query =
                from category in categories
                join product in products on category.Id equals product.CategoryId into productGroup
                from product2 in productGroup.DefaultIfEmpty(new Product { Id = 0, Name = null, CategoryId = null })
                where product2.Id == 0
                select new ProductCategory { ProductId = product2.Id, ProductName = product2.Name, CategoryId = category.Id, CategoryName = category.Name };

            Write(query);
        }

        public static void Test_Linq_FullOuterJoin_ProductCategory_LinqRequest_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            IEnumerable<ProductCategory> queryLeft =
                from product in products
                join category in categories on product.CategoryId equals category.Id into categoryGroup
                from category2 in categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null })
                select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category2.Id, CategoryName = category2.Name };

            IEnumerable<ProductCategory> queryRight =
                from category in categories
                join product in products on category.Id equals product.CategoryId into productGroup
                from product2 in productGroup.DefaultIfEmpty(new Product { Id = 0, Name = null, CategoryId = null })
                where product2.Id == 0
                select new ProductCategory { ProductId = product2.Id, ProductName = product2.Name, CategoryId = category.Id, CategoryName = category.Name };

            IEnumerable<ProductCategory> query = queryLeft.Concat(queryRight);
            Write(query);
        }

        public static void Test_Linq_FullOuterJoin_ProductCategory_LinqRequest_02()
        {
            // DefaultIfEmpty()

            Product[] products = GetProducts();
            Category[] categories = GetCategories();

            IEnumerable<ProductCategory> queryLeft =
                from product in products
                join category in categories on product.CategoryId equals category.Id into categoryGroup
                from category2 in categoryGroup.DefaultIfEmpty()
                select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category2 != null ? category2.Id : 0, CategoryName = category2 != null ? category2.Name : null };

            IEnumerable<ProductCategory> queryRight =
                from category in categories
                join product in products on category.Id equals product.CategoryId into productGroup
                from product2 in productGroup.DefaultIfEmpty()
                where product2 == null
                select new ProductCategory { ProductId = product2 != null ? product2.Id : 0, ProductName = product2 != null ? product2.Name : null, CategoryId = category.Id, CategoryName = category.Name };

            IEnumerable<ProductCategory> query = queryLeft.Concat(queryRight);
            Write(query);
        }

        public static void Test_Linq_FullOuterJoin_ProductCategory_LinqMethod_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();

            //IEnumerable<ProductCategory> queryLeft =
            //    from product in products
            //    join category in categories on product.CategoryId equals category.Id into categoryGroup
            //    from category2 in categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null })
            //    select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category2.Id, CategoryName = category2.Name };

            IEnumerable<ProductCategory> queryLeft =
                products.GroupJoin(
                    categories,                                        // IEnumerable<TInner> inner
                    product => product.CategoryId,                     // Func<TOuter, TKey> outerKeySelector
                    category => category.Id,                           // Func<TInner, TKey> innerKeySelector
                    (product, categoryGroup) =>                        // Func<TOuter, IEnumerable<TInner>, TResult> resultSelector
                        new { product, categoryGroup }
                    ).SelectMany(
                // Func<TSource, IEnumerable<TCollection>> collectionSelector
                        source => source.categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null }),
                // Func<TSource, TCollection, TResult> resultSelector
                        (source, category2) => new ProductCategory { ProductId = source.product.Id, ProductName = source.product.Name, CategoryId = category2.Id, CategoryName = category2.Name }
                    );


            //IEnumerable<ProductCategory> queryRight =
            //    from category in categories
            //    join product in products on category.Id equals product.CategoryId into productGroup
            //    from product2 in productGroup.DefaultIfEmpty(new Product { Id = 0, Name = null, CategoryId = null })
            //    where product2.Id == 0
            //    select new ProductCategory { ProductId = product2.Id, ProductName = product2.Name, CategoryId = category.Id, CategoryName = category.Name };

            IEnumerable<ProductCategory> queryRight =
                categories.GroupJoin(
                    products,                                          // IEnumerable<TInner> inner
                    category => category.Id,                           // Func<TOuter, TKey> outerKeySelector
                    product => product.CategoryId,                     // Func<TInner, TKey> innerKeySelector
                    (category, productGroup) =>                        // Func<TOuter, IEnumerable<TInner>, TResult> resultSelector
                        new { category, productGroup }
                    ).SelectMany(
                        // Func<TSource, IEnumerable<TCollection>> collectionSelector
                        source => source.productGroup.DefaultIfEmpty(new Product { Id = 0, Name = null, CategoryId = null }),
                        // Func<TSource, TCollection, TResult> resultSelector
                        (source, product2) => new ProductCategory { ProductId = product2.Id, ProductName = product2.Name, CategoryId = source.category.Id, CategoryName = source.category.Name }
                    ).Where(source => source.ProductId == 0);

            IEnumerable<ProductCategory> query = queryLeft.Concat(queryRight);
            Write(query);
        }

        public static void Test_Linq_FullOuterJoin_ProductCategory_LinqExtension_01()
        {
            Product[] products = GetProducts();
            Category[] categories = GetCategories();
            //IEnumerable<ProductCategory> queryLeft =
            //    from product in products
            //    join category in categories on product.CategoryId equals category.Id into categoryGroup
            //    from category2 in categoryGroup.DefaultIfEmpty(new Category { Id = 0, Name = null })
            //    select new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category2.Id, CategoryName = category2.Name };

            //IEnumerable<ProductCategory> queryRight =
            //    from category in categories
            //    join product in products on category.Id equals product.CategoryId into productGroup
            //    from product2 in productGroup.DefaultIfEmpty(new Product { Id = 0, Name = null, CategoryId = null })
            //    where product2.Id == 0
            //    select new ProductCategory { ProductId = product2.Id, ProductName = product2.Name, CategoryId = category.Id, CategoryName = category.Name };

            //IEnumerable<ProductCategory> query = queryLeft.Concat(queryRight);

            // new Product { Id = 0, Name = null, CategoryId = null }, new Category { Id = 0, Name = null }, product => product.Id == 0
            //IEnumerable<ProductCategory> query = products.zFullOuterJoin(categories, product => product.CategoryId, category => category.Id,
            //    (product, category) => new ProductCategory
            //        {
            //            ProductId = product != null ? product.Id : 0,
            //            ProductName = product != null ? product.Name : null,
            //            CategoryId = category != null ? category.Id : 0,
            //            CategoryName = category != null ? category.Name : null
            //        });
            IEnumerable<ProductCategory> query = products.zJoin(categories, product => product.CategoryId, category => category.Id,
                (product, category) => new ProductCategory
                {
                    ProductId = product != null ? product.Id : 0,
                    ProductName = product != null ? product.Name : null,
                    CategoryId = category != null ? category.Id : 0,
                    CategoryName = category != null ? category.Name : null
                }, JoinType.FullOuterJoin);

            Write(query);
        }

        public static void Write(IEnumerable<ProductCategory> list)
        {
            Trace.WriteLine(" Product                 Category");
            foreach (ProductCategory value in list)
            {
                //Trace.WriteLine("{0,2} - {1,-15}    {2,2} - {3}", value.ProductId, "\"" + value.ProductName + "\"", value.CategoryId, "\"" + value.CategoryName + "\"");
                if (value.ProductId != 0)
                    Trace.Write("{0,2} - {1,-15}", value.ProductId, "\"" + value.ProductName + "\"");
                else
                    Trace.Write("                    ");
                if (value.CategoryId != 0)
                    Trace.Write("    {0,2} - {1}", value.CategoryId, "\"" + value.CategoryName + "\"");
                Trace.WriteLine();
            }
        }

        public static void Write(IEnumerable<ProductCategories> list)
        {
            Trace.WriteLine(" Product                 Category");
            foreach (ProductCategories value in list)
            {
                //Trace.WriteLine("{0,2} - {1}", value.ProductId, "\"" + value.ProductName + "\"");
                //foreach (Category category in value.Categories)
                //{
                //    Trace.WriteLine("  {0,2} - {1} ", category.Id, "\"" + category.Name + "\"");
                //}
                Trace.Write("{0,2} - {1,-15}", value.ProductId, "\"" + value.ProductName + "\"");
                bool first = true;
                foreach (Category category in value.Categories)
                {
                    if (!first)
                        Trace.Write("                    ");
                    Trace.WriteLine("    {0,2} - {1} ", category.Id, "\"" + category.Name + "\"");
                    first = false;
                }
                if (first)
                    Trace.WriteLine();
            }
        }

        public static void Write(IEnumerable<CategoryProducts> list)
        {
            Trace.WriteLine(" Product                 Category");
            foreach (CategoryProducts value in list)
            {
                //Trace.WriteLine("{0,2} - {1}", value.CategoryId, "\"" + value.CategoryName + "\"");
                //foreach (Product product in value.Products)
                //{
                //    Trace.WriteLine("  {0,2} - {1} ", product.Id, "\"" + product.Name + "\"");
                //}
                bool first = true;
                foreach (Product product in value.Products)
                {
                    Trace.Write("{0,2} - {1,-15}", product.Id, "\"" + product.Name + "\"");
                    if (first)
                        Trace.WriteLine("    {0,2} - {1}", value.CategoryId, "\"" + value.CategoryName + "\"");
                    else
                        Trace.WriteLine();
                    first = false;
                }
                if (first)
                {
                    Trace.WriteLine("                        {0,2} - {1}", value.CategoryId, "\"" + value.CategoryName + "\"");
                }
            }
        }

        public static Product[] GetProducts()
        {
            return new Product[]
                {
                    new Product { Id = 1, Name = "le monde", CategoryId = 2 },
                    new Product { Id = 2, Name = "le figaro", CategoryId = 2 },
                    new Product { Id = 3, Name = "libération", CategoryId = 2 },
                    new Product { Id = 4, Name = "le point", CategoryId = 3 },
                    new Product { Id = 5, Name = "l'espress", CategoryId = 3 },
                    new Product { Id = 6, Name = "marianne", CategoryId = 3 },
                    new Product { Id = 7, Name = "la bible", CategoryId = 1 },
                    new Product { Id = 8, Name = "atlas", CategoryId = 1 },
                    new Product { Id = 9, Name = "histoire", CategoryId = 1 },
                    new Product { Id = 10, Name = "astérix", CategoryId = null },
                    new Product { Id = 11, Name = "alix", CategoryId = null },
                    new Product { Id = 12, Name = "tintin", CategoryId = null }
                };
        }

        public static Category[] GetCategories()
        {
            return new Category[]
                {
                    new Category { Id = 1, Name = "livre" },
                    new Category { Id = 2, Name = "journal" },
                    new Category { Id = 3, Name = "magazine" },
                    new Category { Id = 4, Name = "film" },
                    new Category { Id = 5, Name = "music" },
                    new Category { Id = 6, Name = "image" }
                };
        }
    }

    public class Category
    {
        public int Id;
        public string Name;
    }

    public class Product
    {
        public int Id;
        public string Name;
        public int? CategoryId;
    }

    public class ProductCategory
    {
        public int ProductId;
        public string ProductName;
        public int CategoryId;
        public string CategoryName;
    }

    public class ProductCategories
    {
        public int ProductId;
        public string ProductName;
        public IEnumerable<Category> Categories;
    }

    public class CategoryProducts
    {
        public int CategoryId;
        public string CategoryName;
        public IEnumerable<Product> Products;
    }
}
