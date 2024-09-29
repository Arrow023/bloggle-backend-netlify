namespace Bloggle.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bloggle.DataAcessLayer.BloggleContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Bloggle.DataAcessLayer.BloggleContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Lifestyle" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Food & Cooking" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Travel" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Fashion" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Technology" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Finance" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Parenting" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Health" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Fitness" });
            context.Categories.Add(new BusinessLayer.Category { CategoryName= "Education & Learning" });

            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to create a blog?", Solution = "Click on the 'Create Blog' button, fill in the blog title and content, and submit the form to publish your blog.", Vote = 5 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to update my profile?", Solution = "Click on the 'Update Profile' button, edit your details like name and profile picture, and save the changes.", Vote = 5 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to view categories?", Solution = "Check the left side of the window for a list of categories and click on the one you want to explore.", Vote = 7 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to bookmark a video?", Solution = "Click on the 'Bookmark' button located under or next to the video to save it for later viewing.", Vote = 8 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to access FAQs?", Solution = "Click on the 'FAQ' button from the menu to view commonly asked questions and their answers.", Vote = 5 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to see comments?", Solution = "Click on the 'Comments' button below the post or video to read user comments.", Vote = 50 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to view user profiles?", Solution = "Click on the profile picture of the user to access their profile and view their details.", Vote = 9 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to read a blog?", Solution = "Click on the blog title or image to open and read the full content of the blog post.", Vote = 50 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to create a categorized blog?", Solution = "While creating a blog, choose a category from the dropdown menu before submitting the form.", Vote = 4 });
            context.FAQs.Add(new BusinessLayer.FAQ { Question = "How to like a blog?", Solution = "Click on the 'Like' button below the blog post to show your appreciation.", Vote = 1 });


        }
    }
}
