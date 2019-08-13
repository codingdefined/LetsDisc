[![Build status](https://ci.appveyor.com/api/projects/status/hgyfsn195bhae5ch/branch/master?svg=true)](https://ci.appveyor.com/project/codingdefined/letsdisc/branch/master)
[![codecov](https://codecov.io/gh/codingdefined/LetsDisc/branch/master/graph/badge.svg)](https://codecov.io/gh/codingdefined/LetsDisc)
[![CodeFactor](https://www.codefactor.io/repository/github/codingdefined/letsdisc/badge)](https://www.codefactor.io/repository/github/codingdefined/letsdisc)

# LetsDisc

A StackOverflow on Steem Blockchain, where you can add your programming questions. It will have Code runner and it is on your hand would you like to post on Steem or not. You do not have to have a Steem Account to use it.

### How to Contribute

Make a contribution and create a PR, either as a new feature or a bug fixing.

### Clone

You should have Visual Studio 2017 Installed on your system.

git clone https://github.com/codingdefined/LetsDisc LetsDisc

Open LetsDisc Folder and run LetsDisc.sln. 
Run the Solution to get the API information : http://localhost:21021/swagger/index.html
In Node Terminal (LetsDisc/src/LetsDisc.Web.Host), run npm install and then npm start to run the Angular App

When adding a new class (Make Sure the Default Project is src/LetsDisc.EntityFrameworkCore)

1. First delete the database
2. Run `Remove-Migration` from Package Manager Console
3. Run `Add-Migration LetsDisc-v1` to add the latest migration
4. Run `update-database` to update the database

### Technology Stack

1. .NET CORE 2.0
2. C#
3. JavaScript
4. JQuery
5. SQL
6. Entity Framework Core

### Project Background

The Project is based on https://aspnetboilerplate.com/, where the initial code us taken from it. 
More at https://steemit.com/utopian-io/@codingdefined/getting-started-with-aspnetboilerplate.

## Open Source Code Used

1. https://github.com/benahm/EnhPageDown
