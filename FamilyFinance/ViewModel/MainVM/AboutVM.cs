using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FamilyFinance.ViewModel.MainVM
{
    public class AboutVM : INotifyPropertyChanged
    {
        public Role UserRole { get { return DataManager.UserRole; } }

        private RelayCommand<FlowDocumentScrollViewer> _navigateToAboutProjectCommand;
        public RelayCommand<FlowDocumentScrollViewer> NavigateToAboutProjectCommand =>
            _navigateToAboutProjectCommand ??= new RelayCommand<FlowDocumentScrollViewer>(NavigateToAboutProject);

        private RelayCommand<FlowDocumentScrollViewer> _navigateToProjectDescriptionCommand;
        public RelayCommand<FlowDocumentScrollViewer> NavigateToProjectDescriptionCommand =>
            _navigateToProjectDescriptionCommand ??= new RelayCommand<FlowDocumentScrollViewer>(NavigateToProjectDescription);

        private RelayCommand<FlowDocumentScrollViewer> _navigateToProjectRulesCommand;
        public RelayCommand<FlowDocumentScrollViewer> NavigateToProjectRulesCommand =>
            _navigateToProjectRulesCommand ??= new RelayCommand<FlowDocumentScrollViewer>(NavigateToProjectRules);

        private RelayCommand<FlowDocumentScrollViewer> _navigateToProjectPrivacyPolicyCommand;
        public RelayCommand<FlowDocumentScrollViewer> NavigateToProjectPrivacyPolicyCommand =>
            _navigateToProjectPrivacyPolicyCommand ??= new RelayCommand<FlowDocumentScrollViewer>(NavigateToProjectPrivacyPolicy);

        public event PropertyChangedEventHandler PropertyChanged;

        private void NavigateToAboutProject(FlowDocumentScrollViewer scrollViewer)
        {
            var paragraph = scrollViewer.Document.Blocks.FirstOrDefault(block => block.Name == "AboutProject");
            paragraph?.BringIntoView();
        }

        private void NavigateToProjectDescription(FlowDocumentScrollViewer scrollViewer)
        {
            var paragraph = scrollViewer.Document.Blocks.FirstOrDefault(block => block.Name == "ProjectDescription");
            paragraph?.BringIntoView();
        }

        private void NavigateToProjectRules(FlowDocumentScrollViewer scrollViewer)
        {
            var paragraph = scrollViewer.Document.Blocks.FirstOrDefault(block => block.Name == "ProjectRules");
            paragraph?.BringIntoView();
        }

        private void NavigateToProjectPrivacyPolicy(FlowDocumentScrollViewer scrollViewer)
        {
            var paragraph = scrollViewer.Document.Blocks.FirstOrDefault(block => block.Name == "ProjectPrivacyPolicy");
            paragraph?.BringIntoView();
        }
    }
}
