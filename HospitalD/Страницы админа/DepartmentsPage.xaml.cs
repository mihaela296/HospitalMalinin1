﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HospitalD
{
    public partial class DepartmentsPage : Page
    {
        private readonly HospitalDRmEntities _db = new HospitalDRmEntities();

        public DepartmentsPage()
        {
            InitializeComponent();
            LoadDepartments();
            InitializeFirstLetterFilter();
        }

        private void InitializeFirstLetterFilter()
        {
            FirstLetterFilter.Items.Add(new ComboBoxItem() { Content = "Все" });
            for (char c = 'А'; c <= 'Я'; c++)
            {
                FirstLetterFilter.Items.Add(new ComboBoxItem() { Content = c.ToString() });
            }
            FirstLetterFilter.SelectedIndex = 0;
        }

        private void LoadDepartments()
        {
            UpdateDepartments();
        }

        private void UpdateDepartments()
        {
            // Сначала получаем все данные
            var allDepartments = _db.Departments
                .AsNoTracking()
                .ToList(); // Материализуем запрос

            // Применяем фильтры к данным в памяти
            var filteredDepartments = allDepartments.AsQueryable();

            // Фильтрация по первой букве (теперь работает в памяти)
            if (FirstLetterFilter.SelectedIndex > 0 &&
                FirstLetterFilter.SelectedItem is ComboBoxItem selectedLetterItem)
            {
                string letter = selectedLetterItem.Content.ToString();
                filteredDepartments = filteredDepartments.Where(d =>
                    d.Name.StartsWith(letter, StringComparison.OrdinalIgnoreCase));
            }

            // Фильтрация по названию
            if (!string.IsNullOrWhiteSpace(SearchDepartmentName.Text))
            {
                string searchTerm = SearchDepartmentName.Text.ToLower();
                filteredDepartments = filteredDepartments.Where(d =>
                    d.Name.ToLower().Contains(searchTerm));
            }

            // Сортировка
            switch (SortDepartmentComboBox.SelectedIndex)
            {
                case 0: filteredDepartments = filteredDepartments.OrderBy(d => d.ID_Department); break;
                case 1: filteredDepartments = filteredDepartments.OrderBy(d => d.Name); break;
                case 2: filteredDepartments = filteredDepartments.OrderByDescending(d => d.Name); break;
                case 3: filteredDepartments = filteredDepartments.OrderBy(d => d.ID_Department); break;
                case 4: filteredDepartments = filteredDepartments.OrderByDescending(d => d.ID_Department); break;
            }

            DepartmentsDataGrid.ItemsSource = filteredDepartments.ToList();
        }

        private void FirstLetterFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDepartments();
        }

        private void SearchDepartmentName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDepartments();
        }

        private void SortDepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDepartments();
        }

        private void CleanFilter_OnClick(object sender, RoutedEventArgs e)
        {
            SearchDepartmentName.Text = string.Empty;
            SortDepartmentComboBox.SelectedIndex = 0;
            FirstLetterFilter.SelectedIndex = 0;
            UpdateDepartments();
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (DepartmentsDataGrid.SelectedItem is Departments selectedDepartment)
            {
                NavigationService.Navigate(new AddEditDepartmentPage(selectedDepartment));
            }
            else
            {
                MessageBox.Show("Выберите отделение для редактирования!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditDepartmentPage());
        }

        private void ButtonDel_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DepartmentsDataGrid.SelectedItem is Departments selectedDepartment))
            {
                MessageBox.Show("Выберите отделение для удаления!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить отделение '{selectedDepartment.Name}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                if (_db.Entry(selectedDepartment).State == EntityState.Detached)
                {
                    _db.Departments.Attach(selectedDepartment);
                }

                _db.Departments.Remove(selectedDepartment);
                _db.SaveChanges();
                UpdateDepartments();
                MessageBox.Show("Отделение успешно удалено!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                MessageBox.Show("Невозможно удалить отделение, так как оно связано с другими записями в базе данных.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DepartmentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить логику при необходимости
        }
    }
}