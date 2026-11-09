using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using LeadScoring.Core;
using LeadScoring.Data;
using LeadScoring.Localization;

namespace LeadScoring;

public partial class MainWindow : Window
{
    private readonly ICompanyRepository _repository = new JsonCompanyRepository();
    private readonly ObservableCollection<Company> _companies;
    private readonly ListCollectionView _view;
    private readonly DispatcherTimer _saveTimer;
    private bool _saveErrorShown;

    public MainWindow()
    {
        InitializeComponent();

        MaxScoreText.Text = string.Format(Loc.T("MaxScoreInfo"), ScoringModel.MaxScore);

        // Autosave: any change schedules a save 500 ms later (so typing doesn't write on every key).
        _saveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        _saveTimer.Tick += (_, _) => { _saveTimer.Stop(); Save(); };
        Closing += (_, _) => { _saveTimer.Stop(); Save(); };

        _companies = new ObservableCollection<Company>(LoadCompanies());
        foreach (var company in _companies) company.PropertyChanged += OnCompanyChanged;
        _companies.CollectionChanged += OnCollectionChanged;

        // Default view: best leads first. Clicking any column header changes the sort (AC 5).
        _view = new ListCollectionView(_companies);
        _view.SortDescriptions.Add(new SortDescription(nameof(Company.LeadScore), ListSortDirection.Descending));
        _view.LiveSortingProperties.Add(nameof(Company.LeadScore));
        ScoreColumn.SortDirection = ListSortDirection.Descending;
        CompaniesGrid.ItemsSource = _view;
    }

    private IReadOnlyList<Company> LoadCompanies()
    {
        try
        {
            return _repository.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(string.Format(Loc.T("LoadError"), ex.Message), Loc.T("AppTitle"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return new List<Company>();
        }
    }

    private void Save()
    {
        try
        {
            _repository.Save(_companies);
            _saveErrorShown = false;
        }
        catch (Exception ex)
        {
            if (_saveErrorShown) return; // don't nag on every autosave
            _saveErrorShown = true;
            MessageBox.Show(string.Format(Loc.T("SaveError"), ex.Message), Loc.T("AppTitle"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ScheduleSave()
    {
        _saveTimer.Stop();
        _saveTimer.Start();
    }

    private void OnCompanyChanged(object? sender, PropertyChangedEventArgs e) => ScheduleSave();

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (Company company in e.OldItems) company.PropertyChanged -= OnCompanyChanged;
        if (e.NewItems != null)
            foreach (Company company in e.NewItems) company.PropertyChanged += OnCompanyChanged;
        ScheduleSave();
    }

    // ---- Add ----------------------------------------------------------------------------------

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        CompaniesGrid.CommitEdit(DataGridEditingUnit.Row, true);

        var company = new Company { Name = Loc.T("NewCompanyName") };
        _companies.Add(company);

        CompaniesGrid.SelectedItem = company;
        CompaniesGrid.ScrollIntoView(company);

        // Start editing the name once the new row has been laid out.
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        {
            CompaniesGrid.CurrentCell = new DataGridCellInfo(company, CompaniesGrid.Columns[0]);
            CompaniesGrid.BeginEdit();
        }));
    }

    // ---- Delete -------------------------------------------------------------------------------

    private void DeleteButton_Click(object sender, RoutedEventArgs e) => DeleteSelected();

    private void CompaniesGrid_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Don't hijack Delete while the user is typing in the name cell.
        if (e.Key == Key.Delete && Keyboard.FocusedElement is not TextBox)
        {
            DeleteSelected();
            e.Handled = true;
        }
    }

    private void DeleteSelected()
    {
        var selected = CompaniesGrid.SelectedItems.Cast<Company>().ToList();
        if (selected.Count == 0) return;

        var answer = MessageBox.Show(
            string.Format(Loc.T("ConfirmDeleteText"), selected.Count),
            Loc.T("ConfirmDeleteTitle"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) return;

        CompaniesGrid.CancelEdit();
        foreach (var company in selected) _companies.Remove(company);
    }

    // ---- Sorting ------------------------------------------------------------------------------

    // Optional: keep the list ordered while dropdowns change (rows jump to their new position).
    private void AutoSort_Changed(object sender, RoutedEventArgs e) =>
        _view.IsLiveSorting = AutoSortCheck.IsChecked == true;
}
