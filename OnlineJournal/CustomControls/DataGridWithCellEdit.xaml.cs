using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace OnlineJournal.CustomControls
{
    public partial class DataGridWithCellEdit : UserControl
    {
        private string previousValue;

        public static readonly DependencyProperty CellProperty;
        public static readonly DependencyProperty ItemsSourseProperty;

        public (int, int, int) EditedCell
        {
            get => ((int, int, int))GetValue(CellProperty);
            set => SetValue(CellProperty, value);
        }

        public IEnumerable ItemsSourse
        {
            get => (IEnumerable)GetValue(ItemsSourseProperty);
            set => SetValue(ItemsSourseProperty, value);
        }

        static DataGridWithCellEdit()
        {
            CellProperty = DependencyProperty.Register("EditedCell", typeof((int, int, int)), typeof(DataGridWithCellEdit));
            ItemsSourseProperty = DependencyProperty
                .Register("ItemsSourse", typeof(IEnumerable), typeof(DataGridWithCellEdit), new PropertyMetadata(null, OnItemsSourceChanged));
        }

        public DataGridWithCellEdit()
        {
            InitializeComponent();
            dataGrid.PreparingCellForEdit += DataGrid_PreparingCellForEdit;
            dataGrid.CellEditEnding += DataGrid_CellEditEnding;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridWithCellEdit customDataGrid)
            {
                customDataGrid.OnItemsSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable);
            }
        }

        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            dataGrid.ItemsSource = newValue;
        }

        private void DataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            previousValue = (e.Row.Item as DataRowView)[e.Column.DisplayIndex].ToString();
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
                return;

            if (e.EditingElement is TextBox textBox)
            {
                string text = Regex.Replace(textBox.Text, @"\s+", "");

                if (previousValue != text
                    && (int.TryParse(textBox.Text, out int newValue)
                    || (!string.IsNullOrEmpty(previousValue) && string.IsNullOrEmpty(text))))
                {
                    if ((newValue < 1 || newValue > 100) && !string.IsNullOrEmpty(text))
                        dataGrid.CancelEdit(DataGridEditingUnit.Cell);
                    else
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            newValue = -1;
                            textBox.Text = null;
                        }

                        EditedCell = (e.Row.GetIndex(), e.Column.DisplayIndex, newValue);
                    }
                }
                else
                {
                    dataGrid.CancelEdit(DataGridEditingUnit.Cell);
                }
            }
        }
    }
}
