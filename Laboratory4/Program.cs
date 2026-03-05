using System;
using System.Text;

public class MatrixException : Exception
{
  public MatrixException(string message) : base(message) 
  { 
  }
}

public class MatrixSizeException : MatrixException
{
  public MatrixSizeException() : base("Matrix size mismatch") 
  { 
  }
}

public class MatrixSingularException : MatrixException
{
  public MatrixSingularException() : base("Matrix is singular") 
  { 
  }
}

public class MatrixIndexException : MatrixException
{
  public MatrixIndexException() : base("Index out of range") 
  { 
  }
}

public class SquareMatrix : ICloneable, IComparable<SquareMatrix>
{
  private int size;
  private double[,] data;
  private static Random randomGenerator;

  private static readonly double comparisonEpsilon;
  private static readonly double singularityEpsilon;

  private static readonly int hashCodePrime1;
  private static readonly int hashCodePrime2;
  private static readonly int singleElementMatrixSize;
  private static readonly int twoByTwoMatrixSize;
  private static readonly int startRowForMinor;
  private static readonly int signInitialValue;

  private static readonly double signPositive;
  private static readonly double signNegative;

  static SquareMatrix()
  {
    comparisonEpsilon = 1e-10;
    singularityEpsilon = 1e-10;
    hashCodePrime1 = 17;
    hashCodePrime2 = 31;
    singleElementMatrixSize = 1;
    twoByTwoMatrixSize = 2;
    startRowForMinor = 1;
    signInitialValue = 1;
    signPositive = 1.0;
    signNegative = -1.0;
 
    randomGenerator = new Random();
  }

  public SquareMatrix(int matrixSize)
  {
    if (matrixSize < 0)
    {
      throw new MatrixException("Size cannot be negative");
    }
    
    size = matrixSize;
    data = new double[matrixSize, matrixSize];
  }

  public SquareMatrix(int matrixSize, double minValue, double maxValue) : this(matrixSize)
  {
    double valueRange;
    valueRange = maxValue - minValue;

    for (int rowIndex = 0; rowIndex < matrixSize; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex)
      {
        data[rowIndex, columnIndex] = randomGenerator.NextDouble() * valueRange + minValue;
      }
    }
  }

  public SquareMatrix(SquareMatrix otherMatrix) : this(otherMatrix.size)
  {
    Array.Copy(otherMatrix.data, data, otherMatrix.data.Length);
  }

  public double this[int rowIndex, int columnIndex]
  {
    get
    {
      if (rowIndex < 0 || rowIndex >= size || columnIndex < 0 || columnIndex >= size)
      {
        throw new MatrixIndexException();
      }
      
      return data[rowIndex, columnIndex];
    }
    set
    {
      if (rowIndex < 0 || rowIndex >= size || columnIndex < 0 || columnIndex >= size)
      {
        throw new MatrixIndexException();
      }
      
      data[rowIndex, columnIndex] = value;
    }
  }

  public int Size
  {
    get
    {
      return size;
    }
  }

  public static SquareMatrix operator +(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    if (firstMatrix.size != secondMatrix.size)
    {
      throw new MatrixSizeException();
    }

    SquareMatrix resultMatrix;
    resultMatrix = new SquareMatrix(firstMatrix.size);

    for (int rowIndex = 0; rowIndex < firstMatrix.size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < firstMatrix.size; ++columnIndex)
      {
        resultMatrix[rowIndex, columnIndex] = firstMatrix[rowIndex, columnIndex] + secondMatrix[rowIndex, columnIndex];
      }
    }
    
    return resultMatrix;
  }

  public static SquareMatrix operator -(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    if (firstMatrix.size != secondMatrix.size)
    {
      throw new MatrixSizeException();
    }

    SquareMatrix resultMatrix;
    resultMatrix = new SquareMatrix(firstMatrix.size);

    for (int rowIndex = 0; rowIndex < firstMatrix.size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < firstMatrix.size; ++columnIndex)
      {
        resultMatrix[rowIndex, columnIndex] = firstMatrix[rowIndex, columnIndex] - secondMatrix[rowIndex, columnIndex];
      }
    }
    
    return resultMatrix;
  }

  public static SquareMatrix operator *(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    if (firstMatrix.size != secondMatrix.size)
    {
      throw new MatrixSizeException();
    }

    SquareMatrix resultMatrix;
    resultMatrix = new SquareMatrix(firstMatrix.size);

    for (int rowIndex = 0; rowIndex < firstMatrix.size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < firstMatrix.size; ++columnIndex)
      {
        double elementSum;
        elementSum = 0.0;

        for (int elementIndex = 0; elementIndex < firstMatrix.size; ++elementIndex)
        {
          elementSum = elementSum + firstMatrix[rowIndex, elementIndex] * secondMatrix[elementIndex, columnIndex];
        }
        
        resultMatrix[rowIndex, columnIndex] = elementSum;
      }
    }
    
    return resultMatrix;
  }

  public static bool operator >(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    return firstMatrix.Determinant() > secondMatrix.Determinant();
  }

  public static bool operator <(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    return firstMatrix.Determinant() < secondMatrix.Determinant();
  }

  public static bool operator >=(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    return firstMatrix.Determinant() >= secondMatrix.Determinant();
  }

  public static bool operator <=(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    return firstMatrix.Determinant() <= secondMatrix.Determinant();
  }

  public static bool operator ==(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    if (ReferenceEquals(firstMatrix, null) || ReferenceEquals(secondMatrix, null))
    {
      return ReferenceEquals(firstMatrix, secondMatrix);
    }

    if (firstMatrix.size != secondMatrix.size)
    {
      return false;
    }

    for (int rowIndex = 0; rowIndex < firstMatrix.size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < firstMatrix.size; ++columnIndex)
      {
        if (Math.Abs(firstMatrix[rowIndex, columnIndex] - secondMatrix[rowIndex, columnIndex]) > comparisonEpsilon)
        {
          return false;
        }
      }
    }
    
    return true;
  }

  public static bool operator !=(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    return !(firstMatrix == secondMatrix);
  }

  public static explicit operator double(SquareMatrix matrix)
  {
    return matrix.Determinant();
  }

  public static explicit operator string(SquareMatrix matrix)
  {
    return matrix.ToString();
  }

  public static implicit operator bool(SquareMatrix matrix)
  {
    if (matrix == null)
    {
      return false;
    }

    if (matrix.size == 0)
    {
      return false;
    }

    return Math.Abs(matrix.Determinant()) > singularityEpsilon;
  }

  public double Determinant()
  {
    if (size == 0)
    {
      return 0.0;
    }
    
    return CalculateDeterminant(data, size);
  }

  private double CalculateDeterminant(double[,] matrix, int currentSize)
  {
    if (currentSize == singleElementMatrixSize)
    {
      return matrix[0, 0];
    }

    if (currentSize == twoByTwoMatrixSize)
    {
      return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
    }

    double determinant;
    determinant = 0.0;

    int sign;
    sign = signInitialValue;

    for (int columnIndex = 0; columnIndex < currentSize; ++columnIndex)
    {
      int minorSize;
      minorSize = currentSize - 1;

      double[,] minorMatrix;
      minorMatrix = new double[minorSize, minorSize];

      for (int rowIndex = startRowForMinor; rowIndex < currentSize; ++rowIndex)
      {
        for (int innerColumnIndex = 0; innerColumnIndex < currentSize; ++innerColumnIndex)
        {
          if (innerColumnIndex < columnIndex)
          {
            minorMatrix[rowIndex - 1, innerColumnIndex] = matrix[rowIndex, innerColumnIndex];
          }
          
          if (innerColumnIndex > columnIndex)
          {
            minorMatrix[rowIndex - 1, innerColumnIndex - 1] = matrix[rowIndex, innerColumnIndex];
          }
        }
      }

      determinant = determinant + sign * matrix[0, columnIndex] * CalculateDeterminant(minorMatrix, minorSize);
      sign = -sign;
    }
    
    return determinant;
  }

  public SquareMatrix Inverse()
  {
    double determinant;
    determinant = Determinant();

    if (Math.Abs(determinant) < singularityEpsilon)
    {
      throw new MatrixSingularException();
    }

    if (size == singleElementMatrixSize)
    {
      SquareMatrix resultMatrix;
      resultMatrix = new SquareMatrix(singleElementMatrixSize);
      resultMatrix[0, 0] = signPositive / this[0, 0];
      
      return resultMatrix;
    }

    SquareMatrix inverseMatrix;
    inverseMatrix = new SquareMatrix(size);

    for (int rowIndex = 0; rowIndex < size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < size; ++columnIndex)
      {
        int minorSize;
        minorSize = size - 1;

        double[,] minorMatrix;
        minorMatrix = new double[minorSize, minorSize];

        for (int innerRowIndex = 0; innerRowIndex < size; ++innerRowIndex)
        {
          if (innerRowIndex == rowIndex)
          {
            continue;
          }

          for (int innerColumnIndex = 0; innerColumnIndex < size; ++innerColumnIndex)
          {
            if (innerColumnIndex == columnIndex)
            {
              continue;
            }

            int targetRowIndex;
            targetRowIndex = innerRowIndex;
            if (innerRowIndex > rowIndex)
            {
              targetRowIndex = innerRowIndex - 1;
            }

            int targetColumnIndex;
            targetColumnIndex = innerColumnIndex;
            if (innerColumnIndex > columnIndex)
            {
              targetColumnIndex = innerColumnIndex - 1;
            }

            minorMatrix[targetRowIndex, targetColumnIndex] = this[innerRowIndex, innerColumnIndex];
          }
        }

        SquareMatrix minorSquareMatrix;
        minorSquareMatrix = new SquareMatrix(minorSize);
        minorSquareMatrix.data = minorMatrix;

        double minorDeterminant;
        minorDeterminant = minorSquareMatrix.Determinant();

        int indexSum;
        indexSum = rowIndex + columnIndex;

        int remainder;
        remainder = indexSum % 2;

        double sign;
        sign = signPositive;
        if (remainder != 0)
        {
          sign = signNegative;
        }

        double cofactor;
        cofactor = sign * minorDeterminant;

        inverseMatrix[columnIndex, rowIndex] = cofactor / determinant;
      }
    }
    
    return inverseMatrix;
  }

  public override string ToString()
  {
    if (size == 0)
    {
      return "[]";
    }

    StringBuilder stringBuilder;
    stringBuilder = new StringBuilder();
    stringBuilder.Append("[");

    for (int rowIndex = 0; rowIndex < size; ++rowIndex)
    {
      if (rowIndex > 0)
      {
        stringBuilder.Append(" ");
      }

      stringBuilder.Append("[");

      for (int columnIndex = 0; columnIndex < size; ++columnIndex)
      {
        stringBuilder.Append($"{data[rowIndex, columnIndex],8:F4}");

        if (columnIndex < size - 1)
        {
          stringBuilder.Append(" ");
        }
      }

      stringBuilder.Append("]");

      if (rowIndex < size - 1)
      {
        stringBuilder.Append("\n");
      }
    }

    stringBuilder.Append("]");
    
    return stringBuilder.ToString();
  }

  public int CompareTo(SquareMatrix otherMatrix)
  {
    if (otherMatrix == null)
    {
      return 1;
    }
    
    return Determinant().CompareTo(otherMatrix.Determinant());
  }

  public override bool Equals(object otherObject)
  {
    return this == (otherObject as SquareMatrix);
  }

  public override int GetHashCode()
  {
    int hashCode;
    hashCode = hashCodePrime1;

    for (int rowIndex = 0; rowIndex < size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < size; ++columnIndex)
      {
        hashCode = hashCode * hashCodePrime2 + data[rowIndex, columnIndex].GetHashCode();
      }
    }
    
    return hashCode;
  }

  public object Clone()
  {
    return new SquareMatrix(this);
  }
}

class MatrixCalculator
{
  private SquareMatrix currentMatrix;

  private void PrintMenu()
  {
    Console.WriteLine("\n========== MATRIX CALCULATOR ==========");
    Console.WriteLine("1. New random matrix");
    Console.WriteLine("2. New manual matrix");
    Console.WriteLine("3. Show current matrix");
    Console.WriteLine("4. Add matrices");
    Console.WriteLine("5. Subtract matrices");
    Console.WriteLine("6. Multiply matrices");
    Console.WriteLine("7. Determinant");
    Console.WriteLine("8. Inverse matrix");
    Console.WriteLine("9. Compare matrices");
    Console.WriteLine("10. Hash code");
    Console.WriteLine("11. Clone matrix");
    Console.WriteLine("12. Check if non-singular");
    Console.WriteLine("0. Exit");
    Console.Write("Choice: ");
  }

  private SquareMatrix ReadMatrix(string promptMessage)
  {
    Console.WriteLine(promptMessage);
    Console.WriteLine("1. Random");
    Console.WriteLine("2. Manual");
    Console.Write("Choice: ");

    int userChoice;
    userChoice = int.Parse(Console.ReadLine());

    if (userChoice == 1)
    {
      Console.Write("Size: ");
      int matrixSize;
      matrixSize = int.Parse(Console.ReadLine());

      Console.Write("Min: ");
      double minValue;
      minValue = double.Parse(Console.ReadLine());

      Console.Write("Max: ");
      double maxValue;
      maxValue = double.Parse(Console.ReadLine());

      return new SquareMatrix(matrixSize, minValue, maxValue);
    }
    else
    {
      Console.Write("Size: ");
      int matrixSize;
      matrixSize = int.Parse(Console.ReadLine());

      SquareMatrix newMatrix;
      newMatrix = new SquareMatrix(matrixSize);

      Console.WriteLine("Enter elements:");

      for (int rowIndex = 0; rowIndex < matrixSize; ++rowIndex)
      {
        for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex)
        {
          Console.Write($"[{rowIndex},{columnIndex}]: ");
          newMatrix[rowIndex, columnIndex] = double.Parse(Console.ReadLine());
        }
      }
      
      return newMatrix;
    }
  }

  public void Run()
  {
    Console.WriteLine("Welcome to Matrix Calculator!");

    try
    {
      Console.WriteLine("Create initial matrix:");
      currentMatrix = ReadMatrix("Initial matrix");

      int userChoice;

      do
      {
        PrintMenu();
        userChoice = int.Parse(Console.ReadLine());

        try
        {
          if (userChoice == 1)
          {
            currentMatrix = ReadMatrix("New random matrix");
            Console.WriteLine("Created:\n" + currentMatrix);
          }
          else if (userChoice == 2)
          {
            currentMatrix = ReadMatrix("New manual matrix");
            Console.WriteLine("Created:\n" + currentMatrix);
          }
          else if (userChoice == 3)
          {
            Console.WriteLine("Current matrix:\n" + currentMatrix);
          }
          else if (userChoice == 4)
          {
            SquareMatrix matrixToAdd;
            matrixToAdd = ReadMatrix("Matrix to add:");

            SquareMatrix result;
            result = currentMatrix + matrixToAdd;
            Console.WriteLine("Result:\n" + result);
          }
          else if (userChoice == 5)
          {
            SquareMatrix matrixToSubtract;
            matrixToSubtract = ReadMatrix("Matrix to subtract:");

            SquareMatrix result;
            result = currentMatrix - matrixToSubtract;
            Console.WriteLine("Result:\n" + result);
          }
          else if (userChoice == 6)
          {
            SquareMatrix matrixToMultiply;
            matrixToMultiply = ReadMatrix("Matrix to multiply:");

            SquareMatrix result;
            result = currentMatrix * matrixToMultiply;
            Console.WriteLine("Result:\n" + result);
          }
          else if (userChoice == 7)
          {
            double determinant;
            determinant = currentMatrix.Determinant();
            Console.WriteLine($"Determinant: {determinant:F6}");
          }
          else if (userChoice == 8)
          {
            SquareMatrix inverseMatrix;
            inverseMatrix = currentMatrix.Inverse();
            Console.WriteLine("Inverse:\n" + inverseMatrix);

            SquareMatrix checkMatrix;
            checkMatrix = currentMatrix * inverseMatrix;
            Console.WriteLine("Check:\n" + checkMatrix);
          }
          else if (userChoice == 9)
          {
            SquareMatrix matrixToCompare;
            matrixToCompare = ReadMatrix("Matrix to compare:");

            bool isGreater;
            isGreater = currentMatrix > matrixToCompare;
            Console.WriteLine($"Current > other: {isGreater}");

            bool isLess;
            isLess = currentMatrix < matrixToCompare;
            Console.WriteLine($"Current < other: {isLess}");

            bool isEqual;
            isEqual = currentMatrix == matrixToCompare;
            Console.WriteLine($"Current == other: {isEqual}");

            int comparisonResult;
            comparisonResult = currentMatrix.CompareTo(matrixToCompare);
            Console.WriteLine($"CompareTo: {comparisonResult}");
          }
          else if (userChoice == 10)
          {
            int hashCode;
            hashCode = currentMatrix.GetHashCode();
            Console.WriteLine($"Hash code: {hashCode}");
          }
          else if (userChoice == 11)
          {
            SquareMatrix clonedMatrix;
            clonedMatrix = (SquareMatrix)currentMatrix.Clone();
            Console.WriteLine("Clone:\n" + clonedMatrix);

            bool areEqual;
            areEqual = currentMatrix.Equals(clonedMatrix);
            Console.WriteLine($"Equal: {areEqual}");
          }
          else if (userChoice == 12)
          {
            bool isNonSingular;
            isNonSingular = (bool)currentMatrix;
            Console.WriteLine($"Is non-singular: {isNonSingular}");
          }
          else if (userChoice == 0)
          {
            Console.WriteLine("Goodbye!");
          }
          else
          {
            Console.WriteLine("Invalid choice!");
          }
        }
        catch (Exception error)
        {
          Console.WriteLine($"Error: {error.Message}");
        }

        if (userChoice != 0)
        {
          Console.WriteLine("\nPress Enter...");
          Console.ReadLine();
        }
      } 
      while (userChoice != 0);
    }
    catch (Exception error)
    {
      Console.WriteLine($"Fatal error: {error.Message}");
    }
  }
}

class Program
{
  static void Main()
  {
    Console.OutputEncoding = Encoding.UTF8;

    try
    {
      Console.WriteLine("=== SQUARE MATRIX DEMO ===\n");

      SquareMatrix firstMatrix;
      firstMatrix = new SquareMatrix(3, -5.0, 5.0);
      Console.WriteLine("M1 (random):\n" + firstMatrix);

      SquareMatrix secondMatrix;
      secondMatrix = new SquareMatrix(3);
      secondMatrix[0, 0] = 2.0;
      secondMatrix[0, 1] = 1.0;
      secondMatrix[0, 2] = 1.0;
      secondMatrix[1, 0] = 1.0;
      secondMatrix[1, 1] = 2.0;
      secondMatrix[1, 2] = 1.0;
      secondMatrix[2, 0] = 1.0;
      secondMatrix[2, 1] = 1.0;
      secondMatrix[2, 2] = 2.0;

      Console.WriteLine("M2:\n" + secondMatrix);

      SquareMatrix sumMatrix;
      sumMatrix = firstMatrix + secondMatrix;
      Console.WriteLine("M1 + M2:\n" + sumMatrix);

      SquareMatrix productMatrix;
      productMatrix = firstMatrix * secondMatrix;
      Console.WriteLine("M1 * M2:\n" + productMatrix);

      double detFirst;
      detFirst = firstMatrix.Determinant();
      Console.WriteLine($"Det(M1): {detFirst:F4}");

      double detSecond;
      detSecond = secondMatrix.Determinant();
      Console.WriteLine($"Det(M2): {detSecond:F4}");

      bool isGreater;
      isGreater = firstMatrix > secondMatrix;
      Console.WriteLine($"M1 > M2: {isGreater}");

      double determinantAsDouble;
      determinantAsDouble = (double)secondMatrix;
      Console.WriteLine($"Det as double: {determinantAsDouble:F4}");

      if (secondMatrix)
      {
        Console.WriteLine("M2 is non-singular");
      }

      try
      {
        SquareMatrix inverseMatrix;
        inverseMatrix = secondMatrix.Inverse();
        Console.WriteLine("Inverse of M2:\n" + inverseMatrix);
      }
      catch (MatrixSingularException error)
      {
        Console.WriteLine(error.Message);
      }

      SquareMatrix clonedMatrix;
      clonedMatrix = (SquareMatrix)secondMatrix.Clone();

      bool areEqual;
      areEqual = secondMatrix.Equals(clonedMatrix);
      Console.WriteLine($"Clone equal: {areEqual}");

      Console.WriteLine("\n=== EXCEPTION DEMO ===");

      try
      {
        SquareMatrix smallMatrix;
        smallMatrix = new SquareMatrix(2);

        SquareMatrix largeMatrix;
        largeMatrix = new SquareMatrix(3);

        SquareMatrix invalidResult;
        invalidResult = smallMatrix + largeMatrix;
      }
      catch (MatrixSizeException error)
      {
        Console.WriteLine($"Caught: {error.Message}");
      }

      try
      {
        double invalidElement;
        invalidElement = secondMatrix[5, 5];
      }
      catch (MatrixIndexException error)
      {
        Console.WriteLine($"Caught: {error.Message}");
      }

      Console.WriteLine("\n=== MATRIX CALCULATOR ===");

      MatrixCalculator calculator;
      calculator = new MatrixCalculator();
      calculator.Run();
    }
    catch (Exception error)
    {
      Console.WriteLine($"Error: {error.Message}");
    }
  }
}