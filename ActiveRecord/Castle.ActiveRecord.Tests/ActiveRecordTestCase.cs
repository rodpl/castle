// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;


	[TestFixture]
	public class ActiveRecordTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void SimpleOperations()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			Blog retrieved = blogs[0];
			Assert.IsNotNull( retrieved );

			Assert.AreEqual( blog.Name, retrieved.Name );
			Assert.AreEqual( blog.Author, retrieved.Author );
		}

		[Test]
		public void SimpleOperations2()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Create();

			blogs = Blog.FindAll();
			Assert.AreEqual( blog.Name, blogs[0].Name );
			Assert.AreEqual( blog.Author, blogs[0].Author );

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			blog.Name = "something else1";
			blog.Author = "something else2";
			blog.Update();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );
			Assert.AreEqual( blog.Name, blogs[0].Name );
			Assert.AreEqual( blog.Author, blogs[0].Author );
		}

		[Test]
		public void ComponentAttribute()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Company), typeof(Client), typeof(Firm), typeof(Person) );

			Company.DeleteAll();

			Company company = new Company("Castle Corp.");
			company.Address = new PostalAddress(
				"Embau St., 102", "Sao Paulo", "SP", "040390-060");
			company.Save();

			Company[] companies = Company.FindAll();
			Assert.IsNotNull( companies );
			Assert.AreEqual( 1, companies.Length );

			Company corp = companies[0];
			Assert.IsNotNull( corp.Address );
			Assert.AreEqual( corp.Address.Address, company.Address.Address );
			Assert.AreEqual( corp.Address.City, company.Address.City );
			Assert.AreEqual( corp.Address.State, company.Address.State );
			Assert.AreEqual( corp.Address.ZipCode, company.Address.ZipCode );
		}

		[Test]
		public void RelationsOneToMany()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			Post post1 = new Post(blog, "title1", "contents", "category1");
			Post post2 = new Post(blog, "title2", "contents", "category2");

			post1.Save();
			post2.Save();

			blog = Blog.Find(blog.Id);

			Assert.IsNotNull(blog);
			Assert.IsNotNull( blog.Posts, "posts collection is null" );
			Assert.AreEqual(2, blog.Posts.Count);

			foreach(Post post in blog.Posts)
			{
				Assert.AreEqual( blog.Id, post.Blog.Id );
			}
		}

		[Test]
		public void RelationsOneToManyWithWhereAndOrder()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			Post post1 = new Post(blog, "title1", "contents", "category1");
			Post post2 = new Post(blog, "title2", "contents", "category2");
			Post post3 = new Post(blog, "title3", "contents", "category3");

			post1.Save();
			System.Threading.Thread.Sleep(1000); // Its a smalldatetime (small precision)
			post2.Save();
			System.Threading.Thread.Sleep(1000); // Its a smalldatetime (small precision)
			post3.Published = true;
			post3.Save();

			blog = Blog.Find(blog.Id);

			Assert.IsNotNull(blog);
			Assert.AreEqual(2, blog.UnPublishedPosts.Count);
			Assert.AreEqual(1, blog.PublishedPosts.Count);

			Assert.AreEqual(3, blog.RecentPosts.Count);
			Assert.AreEqual( post3.Id, (blog.RecentPosts[0] as Post).Id );
			Assert.AreEqual( post2.Id, (blog.RecentPosts[1] as Post).Id );
			Assert.AreEqual( post1.Id, (blog.RecentPosts[2] as Post).Id );
		}

		[Test]
		public void RelationsOneToOne()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Employee), typeof(Award) );

			Award.DeleteAll();
			Employee.DeleteAll();

			Employee emp = new Employee();
			emp.FirstName = "john";
			emp.LastName = "doe";
			emp.Save();

			Assert.AreEqual(1, Employee.FindAll().Length );

			Award award = new Award(emp);
			award.Description = "Invisible employee";
			award.Save();

			Assert.AreEqual(1, Award.FindAll().Length );

			Employee emp2 = Employee.Find(emp.ID);
			Assert.IsNotNull(emp2);
			Assert.IsNotNull(emp2.Award);
			Assert.AreEqual(emp.FirstName, emp2.FirstName);
			Assert.AreEqual(emp.LastName, emp2.LastName);
			Assert.AreEqual(award.Description, emp2.Award.Description);
		}

		[Test]
		public void FindLoad()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = Blog.Find(0);
			Assert.IsNull(blog);
		}

		[Test]
		public void SaveUpdate()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			blog.Name = "Something else";
			blog.Author = "changed too";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			Assert.AreEqual( blog.Name, blogs[0].Name );
			Assert.AreEqual( blog.Author, blogs[0].Author );
		}

		[Test]
		public void Delete()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			blog.Delete();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );
		}

		[Test]
		public void ExecuteAndCallback()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			blog.CustomAction();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );
		}
	}
}
