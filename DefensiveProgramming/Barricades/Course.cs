using System.Diagnostics;

namespace Barricades;

class Course(string name)
{
  public string Name { get; private set; } = name;

  //Consider why is dictionary private variable, and why not a property!    
  //indexer serves as a barricade
  
  private Dictionary<string, int> grades = new Dictionary<string, int>();

  /// <summary>
  /// Does the student have a grade?
  /// </summary>
  /// <param name="name">Student's name</param>
  public bool HasGrade(string name) => grades.ContainsKey(name);

  /// <summary>
  /// Assign the new grade to the student.
  /// Old value (if had existed) is replaced.    
  /// </summary>
  /// <param name="name">Student's name</param>
  /// <returns>The student's grade</returns>
  /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown if the student does not have a grade. Check HasGrade first.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException">Thrown if value is not valid grade [1-5]</exception>
  public int this[string name]
  {
    get
    {      
      //what if there is no student's name in the dictionary
      //the caller should have asked HasGrade first, so the dictionary throws KeyNotFoundException instead of returning a special value (-1)
      return grades[name];
    }
    set
    {
      ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
      ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 5);

      grades[name] = value;
    }
  }

  public double AverageGrade()
  {
    int sum = 0;
    foreach (var pair in grades)
    {
      int grade = pair.Value;
      Debug.Assert(grade >= 1 && grade <= 5,
                   $"Invalid grade in dictionary {pair.Key} = {pair.Value}. This should never happen!");
      sum += grade;
    }
    return grades.Count > 0 ? (double)sum / grades.Count : 0;
  }
}