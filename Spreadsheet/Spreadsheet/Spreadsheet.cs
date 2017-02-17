using Dependencies;
using Formulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SS
{

    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string s is a valid cell name if and only if it consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names.  On the other hand, 
    /// "Z", "X07", and "hello" are not valid cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {



        /// <summary>
        /// The cell class is a abstraction in a spreadsheet, that contains 
        /// </summary>
        private class Cell
        {
            // cell name
            private string cellName { get; set; }

            /// <summary>
            /// Cell's content
            /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
            /// contents is an empty string, we say that the cell is empty. 
            /// </summary>
            public object content { get; set; }
            /// <summary>
            /// Cell's value
            /// If a cell's contents is a string, its value is that string.
            /// 
            /// If a cell's contents is a double, its value is that double.
            /// 
            /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
            /// The value of a Formula, of course, can depend on the values of variables.  The value 
            /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
            /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
            /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
            /// is a double, as specified in Formula.Evaluate.
            /// </summary>
            //public object value { get; set; }

            /// <summary>
            /// constractor that create an empty cell
            /// </summary>
            /// <param name="name"></param>
            public Cell(string name)
            {
                this.cellName = name;
                this.content = "";
            }
        }

        /// <summary>
        /// A DependencyGraph class is used to keep track of the relationships among spreadsheet cells
        /// </summary>
        private DependencyGraph dependencyGraph;

        /// <summary>
        /// A Dictionary containing all the cells in spreadsheet
        /// </summary>
        private Dictionary<string, Cell> cellSet;


        /// <summary>
        /// Zero argument constructor for spreadsheet
        /// </summary>
        public Spreadsheet()
        {
            dependencyGraph = new DependencyGraph();
            cellSet = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<String> GetNamesOfAllNonemptyCells()
        {
            foreach(string str in cellSet.Keys)
            {
                if (!cellSet[str].content.Equals(""))
                {
                    yield return str;
                }
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(String name)

        {
            if(name == null || !isValidCellName(name))
            {
                throw new InvalidNameException();
            }
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
            }

            return cellSet[name].content;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<String> SetCellContents(String name, double number)
        {
            // if name is null or invalid, throws an InvalidNameException.
            if (name == null || !isValidCellName(name))
            {
                throw new InvalidNameException();
            }

            // add cell if cell is not exited
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
            }

            // remove dependees for new content
            dependencyGraph.ReplaceDependents(name,new HashSet<string>());

            //using result set to put name plus the names of all other cells whose value depends
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));

            // set cell content and value
            cellSet[name].content = number;

            return result;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<String> SetCellContents(String name, String text)
        {
            //If text is null, throws an ArgumentNullException.
            if (text == null)
            {
                throw new ArgumentNullException();
            }

            //if name is null or invalid, throws an InvalidNameException.
            if (name == null || !isValidCellName(name))
            {
                throw new InvalidNameException();
            }

            //add the cell if the cell is not exist yet
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
            }

            // remove dependees for new content
            dependencyGraph.ReplaceDependents(name, new HashSet<string>());

            //using result set to put name plus the names of all other cells whose value depends
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));

            //set content and value
            cellSet[name].content = text;

            return result;
        }

        /// <summary>
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<String> SetCellContents(String name, Formula formula)
        {
            //if name is null or invalid, throws an InvalidNameException.
            if (name == null || !isValidCellName(name))
            {
                throw new InvalidNameException();
            }

            //if name is null or invalid, throws an InvalidNameException.
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
            }

            // remove dependees for new content
            dependencyGraph.ReplaceDependents(name, formula.GetVariables());

            //using result set to put name plus the names of all other cells whose value depends
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));
            cellSet[name].content = formula;

            return result;
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<String> GetDirectDependents(String name)
        {
            // If name is null, throws an ArgumentNullException
            if (name == null)
            {
                throw new ArgumentNullException();
            }

            // if name isn't a valid cell name, throws an InvalidNameException.
            if (!isValidCellName(name))
            {
                throw new InvalidNameException();
            }

            // unit dependees and dependents of one cell
            ISet<string> result = new HashSet<string>(dependencyGraph.GetDependees(name));

            return result;
        }

        /// <summary>
        /// Helper method that return ture if cell name is valid, return false if cell name is not valid.
        /// Valid cell name consists of one or more letters, followed by a non-zero digit, followed by 
        /// zero or more digits.
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        private bool isValidCellName(string cellName)
        {
            return Regex.IsMatch(cellName, "^[a-zA-z]{1,2}[1-9]{1}[0-9]*$");
        }



    }
}
