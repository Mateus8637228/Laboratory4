
using System;
using System.Text;

public class MatrixException : Exception
{
  public MatrixException(string message) : base(message) { }
}

public class MatrixSizeException : MatrixException
{
  public MatrixSizeException() : base("Matrix size mismatch") { }
}

public class MatrixSingularException : MatrixException
{
  public MatrixSingularException() : base("Matrix is singular") { }
}

public class MatrixIndexException : MatrixException
{
  public MatrixIndexException() : base("Index out of range") { }
}

public class SquareMatrix : ICloneable, IComparable<SquareMatrix>
{
  private int size;
  private double[,] data;
  private static Random randomGenerator = new Random();

  public SquareMatrix(int matrixSize)
  {
    if (matrixSize < 0) throw new MatrixException("Size cannot be negative");
    size = matrixSize;
    data = new double[matrixSize, matrixSize];
  }

  public SquareMatrix(int matrixSize, double minValue, double maxValue) : this(matrixSize)
  {
    double valueRange = maxValue - minValue;
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
        throw new MatrixIndexException();
      return data[rowIndex, columnIndex];
    }
    set
    {
      if (rowIndex < 0 || rowIndex >= size || columnIndex < 0 || columnIndex >= size)
        throw new MatrixIndexException();
      data[rowIndex, columnIndex] = value;
    }
  }

  public int Size { get { return size; } }

  public static SquareMatrix operator +(SquareMatrix firstMatrix, SquareMatrix secondMatrix)
  {
    if (firstMatrix.size != secondMatrix.size) throw new MatrixSizeException();
    SquareMatrix resultMatrix = new SquareMatrix(firstMatrix.size);
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
    if (firstMatrix.size != secondMatrix.size) throw new MatrixSizeException();
    SquareMatrix resultMatrix = new SquareMatrix(firstMatrix.size);
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
    if (firstMatrix.size != secondMatrix.size) throw new MatrixSizeException();
    SquareMatrix resultMatrix = new SquareMatrix(firstMatrix.size);
    for (int rowIndex = 0; rowIndex < firstMatrix.size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < firstMatrix.size; ++columnIndex)
      {
        double elementSum = 0.0;
        for (int elementIndex = 0; elementIndex < firstMatrix.size; ++elementIndex)
        {
          elementSum += firstMatrix[rowIndex, elementIndex] * secondMatrix[elementIndex, columnIndex];
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
      return ReferenceEquals(firstMatrix, secondMatrix);
    if (firstMatrix.size != secondMatrix.size) return false;
    double epsilon = 1e-10;
    for (int rowIndex = 0; rowIndex < firstMatrix.size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < firstMatrix.size; ++columnIndex)
      {
        if (Math.Abs(firstMatrix[rowIndex, columnIndex] - secondMatrix[rowIndex, columnIndex]) > epsilon)
          return false;
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
    if (matrix == null) return false;
    if (matrix.size == 0) return false;
    return Math.Abs(matrix.Determinant()) > 1e-10;
  }

  public double Determinant()
  {
    if (size == 0) return 0.0;
    return CalculateDeterminant(data, size);
  }

  private double CalculateDeterminant(double[,] matrix, int currentSize)
  {
    if (currentSize == 1) return matrix[0, 0];
    if (currentSize == 2)
    {
      return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
    }
    double determinant = 0.0;
    int sign = 1;
    for (int columnIndex = 0; columnIndex < currentSize; ++columnIndex)
    {
      int minorSize = currentSize - 1;
      double[,] minorMatrix = new double[minorSize, minorSize];
      for (int rowIndex = 1; rowIndex < currentSize; ++rowIndex)
      {
        for (int innerColumnIndex = 0; innerColumnIndex < currentSize; ++innerColumnIndex)
        {
          if (innerColumnIndex < columnIndex)
            minorMatrix[rowIndex - 1, innerColumnIndex] = matrix[rowIndex, innerColumnIndex];
          if (innerColumnIndex > columnIndex)
            minorMatrix[rowIndex - 1, innerColumnIndex - 1] = matrix[rowIndex, innerColumnIndex];
        }
      }
      determinant += sign * matrix[0, columnIndex] * CalculateDeterminant(minorMatrix, minorSize);
      sign = -sign;
    }
    return determinant;
  }

  public SquareMatrix Inverse()
  {
    double determinant = Determinant();
    if (Math.Abs(determinant) < 1e-10) throw new MatrixSingularException();
    if (size == 1)
    {
      SquareMatrix resultMatrix = new SquareMatrix(1);
      resultMatrix[0, 0] = 1.0 / this[0, 0];
      return resultMatrix;
    }
    SquareMatrix inverseMatrix = new SquareMatrix(size);
    for (int rowIndex = 0; rowIndex < size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < size; ++columnIndex)
      {
        int minorSize = size - 1;
        double[,] minorMatrix = new double[minorSize, minorSize];
        for (int innerRowIndex = 0; innerRowIndex < size; ++innerRowIndex)
        {
          if (innerRowIndex == rowIndex) continue;
          for (int innerColumnIndex = 0; innerColumnIndex < size; ++innerColumnIndex)
          {
            if (innerColumnIndex == columnIndex) continue;
            int targetRowIndex = innerRowIndex;
            if (innerRowIndex > rowIndex) targetRowIndex = innerRowIndex - 1;
            int targetColumnIndex = innerColumnIndex;
            if (innerColumnIndex > columnIndex) targetColumnIndex = innerColumnIndex - 1;
            minorMatrix[targetRowIndex, targetColumnIndex] = this[innerRowIndex, innerColumnIndex];
          }
        }
        SquareMatrix minorSquareMatrix = new SquareMatrix(minorSize);
        minorSquareMatrix.data = minorMatrix;
        double minorDeterminant = minorSquareMatrix.Determinant();
        int indexSum = rowIndex + columnIndex;
        int remainder = indexSum % 2;
        double sign = 1.0;
        if (remainder != 0) sign = -1.0;
        double cofactor = sign * minorDeterminant;
        inverseMatrix[columnIndex, rowIndex] = cofactor / determinant;
      }
    }
    return inverseMatrix;
  }

  public override string ToString()
  {
    if (size == 0) return "[]";
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("[");
    for (int rowIndex = 0; rowIndex < size; ++rowIndex)
    {
      if (rowIndex > 0) stringBuilder.Append(" ");
      stringBuilder.Append("[");
      for (int columnIndex = 0; columnIndex < size; ++columnIndex)
      {
        stringBuilder.Append($"{data[rowIndex, columnIndex],8:F4}");
        if (columnIndex < size - 1) stringBuilder.Append(" ");
      }
      stringBuilder.Append("]");
      if (rowIndex < size - 1) stringBuilder.Append("\n");
    }
    stringBuilder.Append("]");
    return stringBuilder.ToString();
  }

  public int CompareTo(SquareMatrix otherMatrix)
  {
    if (otherMatrix == null) return 1;
    return Determinant().CompareTo(otherMatrix.Determinant());
  }

  public override bool Equals(object otherObject)
  {
    return this == (otherObject as SquareMatrix);
  }

  public override int GetHashCode()
  {
    int hashCode = 17;
    for (int rowIndex = 0; rowIndex < size; ++rowIndex)
    {
      for (int columnIndex = 0; columnIndex < size; ++columnIndex)
      {
        hashCode = hashCode * 31 + data[rowIndex, columnIndex].GetHashCode();
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
    int userChoice = int.Parse(Console.ReadLine());
    if (userChoice == 1)
    {
      Console.Write("Size: ");
      int matrixSize = int.Parse(Console.ReadLine());
      Console.Write("Min: ");
      double minValue = double.Parse(Console.ReadLine());
      Console.Write("Max: ");
      double maxValue = double.Parse(Console.ReadLine());
      return new SquareMatrix(matrixSize, minValue, maxValue);
    }
    else
    {
      Console.Write("Size: ");
      int matrixSize = int.Parse(Console.ReadLine());
      SquareMatrix newMatrix = new SquareMatrix(matrixSize);
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
            SquareMatrix matrixToAdd = ReadMatrix("Matrix to add:");
            Console.WriteLine("Result:\n" + (currentMatrix + matrixToAdd));
          }
          else if (userChoice == 5)
          {
            SquareMatrix matrixToSubtract = ReadMatrix("Matrix to subtract:");
            Console.WriteLine("Result:\n" + (currentMatrix - matrixToSubtract));
          }
          else if (userChoice == 6)
          {
            SquareMatrix matrixToMultiply = ReadMatrix("Matrix to multiply:");
            Console.WriteLine("Result:\n" + (currentMatrix * matrixToMultiply));
          }
          else if (userChoice == 7)
          {
            Console.WriteLine($"Determinant: {currentMatrix.Determinant():F6}");
          }
          else if (userChoice == 8)
          {
            SquareMatrix inverseMatrix = currentMatrix.Inverse();
            Console.WriteLine("Inverse:\n" + inverseMatrix);
            Console.WriteLine("Check:\n" + (currentMatrix * inverseMatrix));
          }
          else if (userChoice == 9)
          {
            SquareMatrix matrixToCompare = ReadMatrix("Matrix to compare:");
            Console.WriteLine($"Current > other: {currentMatrix > matrixToCompare}");
            Console.WriteLine($"Current < other: {currentMatrix < matrixToCompare}");
            Console.WriteLine($"Current == other: {currentMatrix == matrixToCompare}");
            Console.WriteLine($"CompareTo: {currentMatrix.CompareTo(matrixToCompare)}");
          }
          else if (userChoice == 10)
          {
            Console.WriteLine($"Hash code: {currentMatrix.GetHashCode()}");
          }
          else if (userChoice == 11)
          {
            SquareMatrix clonedMatrix = (SquareMatrix)currentMatrix.Clone();
            Console.WriteLine("Clone:\n" + clonedMatrix);
            Console.WriteLine($"Equal: {currentMatrix.Equals(clonedMatrix)}");
          }
          else if (userChoice == 12)
          {
            Console.WriteLine($"Is non-singular: {(bool)currentMatrix}");
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
      } while (userChoice != 0);
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
      SquareMatrix firstMatrix = new SquareMatrix(3, -5.0, 5.0);
      Console.WriteLine("M1 (random):\n" + firstMatrix);
      SquareMatrix secondMatrix = new SquareMatrix(3);
      secondMatrix[0, 0] = 2.0; secondMatrix[0, 1] = 1.0; secondMatrix[0, 2] = 1.0;
      secondMatrix[1, 0] = 1.0; secondMatrix[1, 1] = 2.0; secondMatrix[1, 2] = 1.0;
      secondMatrix[2, 0] = 1.0; secondMatrix[2, 1] = 1.0; secondMatrix[2, 2] = 2.0;
      Console.WriteLine("M2:\n" + secondMatrix);
      Console.WriteLine("M1 + M2:\n" + (firstMatrix + secondMatrix));
      Console.WriteLine("M1 * M2:\n" + (firstMatrix * secondMatrix));
      Console.WriteLine($"Det(M1): {firstMatrix.Determinant():F4}");
      Console.WriteLine($"Det(M2): {secondMatrix.Determinant():F4}");
      Console.WriteLine($"M1 > M2: {firstMatrix > secondMatrix}");
      double determinantAsDouble = (double)secondMatrix;
      Console.WriteLine($"Det as double: {determinantAsDouble:F4}");
      if (secondMatrix) Console.WriteLine("M2 is non-singular");
      try
      {
        SquareMatrix inverseMatrix = secondMatrix.Inverse();
        Console.WriteLine("Inverse of M2:\n" + inverseMatrix);
      }
      catch (MatrixSingularException error)
      {
        Console.WriteLine(error.Message);
      }
      SquareMatrix clonedMatrix = (SquareMatrix)secondMatrix.Clone();
      Console.WriteLine($"Clone equal: {secondMatrix.Equals(clonedMatrix)}");
      Console.WriteLine("\n=== EXCEPTION DEMO ===");
      try
      {
        SquareMatrix smallMatrix = new SquareMatrix(2);
        SquareMatrix largeMatrix = new SquareMatrix(3);
        SquareMatrix invalidResult = smallMatrix + largeMatrix;
      }
      catch (MatrixSizeException error)
      {
        Console.WriteLine($"Caught: {error.Message}");
      }
      try
      {
        double invalidElement = secondMatrix[5, 5];
      }
      catch (MatrixIndexException error)
      {
        Console.WriteLine($"Caught: {error.Message}");
      }
      Console.WriteLine("\n=== MATRIX CALCULATOR ===");
      MatrixCalculator calculator = new MatrixCalculator();
      calculator.Run();
    }
    catch (Exception error)
    {
      Console.WriteLine($"Error: {error.Message}");
    }
  }
}