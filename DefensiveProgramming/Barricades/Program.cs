using Barricades;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Course c = new Course("PI");
c["Pero"] = 2;
c["Ana"] = 5;
c["Ivan"] = 5;
c["Marko"] = 1;
c["Luka"] = 3;
//c["Marija"] = -7; //should throw an exception
Console.WriteLine("Average grade: " + c.AverageGrade());
string student = "Šime";
if (c.HasGrade(student))
{
  Console.WriteLine($"{student} has grade: {c[student]}");
}
else
{
  Console.WriteLine($"{student} has no grade");
}
