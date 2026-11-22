using System.Diagnostics;

namespace Barricades;

/// <summary>
/// The same Course and grades example as Course in this project (barricades),
/// but written as a contract (Design by Contract).
///
/// Who is responsible for what:
///   - Precondition  = obligation of the CALLER. A violation is a bug in the calling code,
///                     so the member refuses to work (exception, also in the Release build).
///   - Postcondition = obligation of the MEMBER. A violation is a bug in this class (Debug.Assert).
///   - Invariant     = holds for every Course object before and after each public member (Debug.Assert).
///
/// Class invariant:
///   1. Name is not empty.
///   2. Every stored grade is within [MinGrade, MaxGrade].
/// </summary>
class CourseByContract
{
  public const int MinGrade = 1;
  public const int MaxGrade = 5;

  private readonly Dictionary<string, int> grades = new();

  /// <summary>
  /// Precondition:  name is not null, empty or white space.
  /// Postcondition: the course has the given name and no grades.
  /// </summary>
  public CourseByContract(string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);   // precondition

    Name = name;

    Debug.Assert(grades.Count == 0);                   // postcondition
    CheckInvariant();
  }

  public string Name { get; }

  /// <summary>Number of students that have a grade.</summary>
  public int Count => grades.Count;

  /// <summary>
  /// Precondition:  student is not null, empty or white space.
  /// Postcondition: none. This is a query, it does not change the state.
  /// </summary>
  public bool HasGrade(string student)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(student);   // precondition
    return grades.ContainsKey(student);
  }

  /// <summary>
  /// The grade of the student.
  ///
  /// get - Precondition:  student has a grade (the caller can check it with HasGrade).
  ///       Postcondition: the result is within [MinGrade, MaxGrade].
  ///       Course (barricades) answers an unknown student with KeyNotFoundException from the dictionary.
  ///       Here it is a stated precondition: the caller has to ask HasGrade first, otherwise it is the caller's bug.
  ///
  /// set - Precondition:  student is not empty; value is within [MinGrade, MaxGrade].
  ///       Postcondition: the student has the grade; the old grade (if any) is replaced;
  ///                      Count grows by one for a new student and stays the same otherwise.
  /// </summary>
  public int this[string student]
  {
    get
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(student);                          // precondition

      if (!grades.TryGetValue(student, out int grade))                             // precondition
      {
        throw new InvalidOperationException($"{student} has no grade. Check HasGrade first.");
      }

      Debug.Assert(grade >= MinGrade && grade <= MaxGrade);                        // postcondition
      return grade;
    }
    set
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(student);                          // precondition
      ArgumentOutOfRangeException.ThrowIfLessThan(value, MinGrade);                // precondition
      ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxGrade);             // precondition

      // Eiffel has the keyword "old" for this. In C# we remember the old values ourselves.
      int countBefore = grades.Count;
      bool isNewStudent = !grades.ContainsKey(student);

      grades[student] = value;

      Debug.Assert(grades[student] == value);                                      // postcondition
      Debug.Assert(grades.Count == countBefore + (isNewStudent ? 1 : 0));         // postcondition
      CheckInvariant();
    }
  }

  /// <summary>
  /// Precondition:  student has a grade.
  /// Postcondition: student has no grade; Count is smaller by one.
  /// </summary>
  public void RemoveGrade(string student)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(student);                            // precondition

    if (!grades.ContainsKey(student))                                              // precondition
    {
      throw new InvalidOperationException($"{student} has no grade. Check HasGrade first.");
    }

    int countBefore = grades.Count;

    grades.Remove(student);

    Debug.Assert(!grades.ContainsKey(student));                                    // postcondition
    Debug.Assert(grades.Count == countBefore - 1);                                 // postcondition
    CheckInvariant();
  }

  /// <summary>
  /// Precondition:  the course has at least one grade (Count > 0).
  /// Postcondition: the result is within [MinGrade, MaxGrade].
  /// Course (barricades) returns 0 for an empty course. The average of nothing is not defined,
  /// so here the caller must not ask for it.
  /// </summary>
  public double AverageGrade()
  {
    if (grades.Count == 0)                                                         // precondition
    {
      throw new InvalidOperationException("The average is not defined for a course without grades.");
    }

    CheckInvariant();                                                              // invariant holds before the call

    double average = grades.Values.Average();

    Debug.Assert(average >= MinGrade && average <= MaxGrade,
                 $"Average {average} is outside [{MinGrade}, {MaxGrade}]. This should never happen!");   // postcondition
    return average;
  }

  // The class invariant. The call (and the body) disappears from the Release build.
  [Conditional("DEBUG")]
  private void CheckInvariant()
  {
    Debug.Assert(!string.IsNullOrWhiteSpace(Name), "Course name must not be empty");

    foreach (var (student, grade) in grades)
    {
      Debug.Assert(grade >= MinGrade && grade <= MaxGrade,
                   $"Invalid grade in dictionary {student} = {grade}. This should never happen!");
    }
  }
}
