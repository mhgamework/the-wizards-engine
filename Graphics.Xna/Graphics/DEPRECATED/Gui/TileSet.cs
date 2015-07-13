namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.DEPRECATED.Gui
{
    public class TileSet
    {
        private int left;

        public int Left
        {
            get { return left; }
            set
            {
                left = value;
                CalculateColumnLefts();
            }
        }

        private int top;

        public int Top
        {
            get { return top; }
            set
            {
                top = value;
                CaluclateRowHeights();
            }
        }



        private int[] columnWidths;

        public int[] ColumnWidths
        {
            get { return columnWidths; }
            set
            {
                columnWidths = value;
                CalculateColumnLefts();
            }
        }

        private int[] rowHeights;

        public int[] RowHeights
        {
            get { return rowHeights; }
            set
            {
                rowHeights = value;
                CaluclateRowHeights();
            }
        }

        private int[] colLefts;
        private int[] rowTops;

        private void CalculateColumnLefts()
        {
            colLefts = new int[ columnWidths.Length ];

            int iLeft = left;

            for ( int i = 0; i < columnWidths.Length; i++ )
            {
                colLefts[ i ] = iLeft;
                iLeft += columnWidths[ i ];
            }
        }

        private void CaluclateRowHeights()
        {
            rowTops = new int[ rowHeights.Length ];

            int iTop = top;

            for ( int i = 0; i < rowHeights.Length; i++ )
            {
                rowTops[ i ] = iTop;
                iTop += rowHeights[ i ];
            }
        }


        public TileSet(int nLeft, int nTop, int[] cols, int[] rows )
        {
            left = nLeft;
            top = nTop;
            ColumnWidths = cols;
            RowHeights = rows;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row">0-based row index</param>
        /// <param name="column">0-base column index</param>
        /// <returns></returns>
        public Microsoft.Xna.Framework.Rectangle GetTileRectangle( int row, int column )
        {
            return new Microsoft.Xna.Framework.Rectangle( colLefts[ column ], rowTops[ row ], columnWidths[ column ], rowHeights[ row ] );
        }


    }
}
