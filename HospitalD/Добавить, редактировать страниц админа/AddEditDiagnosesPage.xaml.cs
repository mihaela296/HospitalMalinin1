﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HospitalD
{
    public partial class AddEditDiagnosesPage : Page
    {
        private Diagnoses _currentDiagnosis;
        private HospitalDRmEntities _db;

        public AddEditDiagnosesPage(Diagnoses selectedDiagnosis = null)
        {
            InitializeComponent();

            // Создаем новый контекст для страницы
            _db = new HospitalDRmEntities();

            if (selectedDiagnosis != null)
            {
                // Загружаем диагноз с новым контекстом
                _currentDiagnosis = _db.Diagnoses.Find(selectedDiagnosis.ID_Diagnosis);
                TitleTextBlock.Text = "Редактирование диагноза";
            }
            else
            {
                _currentDiagnosis = new Diagnoses();
            }

            LoadData();
        }

        private void LoadData()
        {
            // Загружаем отделения
            DepartmentComboBox.ItemsSource = _db.Departments.ToList();

            if (_currentDiagnosis.ID_Diagnosis != 0)
            {
                NameTextBox.Text = _currentDiagnosis.Name;
                // Убедимся, что отделение загружено
                if (_currentDiagnosis.ID_Department > 0)
                {
                    DepartmentComboBox.SelectedValue = _currentDiagnosis.ID_Department;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                _currentDiagnosis.Name = NameTextBox.Text.Trim();
                _currentDiagnosis.ID_Department = (int)DepartmentComboBox.SelectedValue;

                if (_currentDiagnosis.ID_Diagnosis == 0)
                {
                    _db.Diagnoses.Add(_currentDiagnosis);
                }

                _db.SaveChanges();

                MessageBox.Show("Данные сохранены успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название диагноза!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (DepartmentComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите отделение!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}