// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class FeesViewModel : PopupShellBase
    {
        private ObservableCollection<TimeGoalItem> _timeGoals;

        public ObservableCollection<TimeGoalItem> TimeGoals
        {
            get
            {
                if (_timeGoals == null)
                    _timeGoals = new ObservableCollection<TimeGoalItem>();
                return _timeGoals;
            }
            set
            {
                _timeGoals = value;
                RaisePropertyChanged("TimeGoals");
            }
        }

        private double _defaultFee = 0.001;
        private double _otherFee = 0.00001024;
        private double _tradeFee = 0.001;

        public double TradeFee
        {
            get { return _tradeFee; }
            set { _tradeFee = value; RaisePropertyChanged("TradeFee"); }
        }


        public override void Init()
        {
            base.Init();

            AppSettingConfig.Default.AppConfig.TimeGoalItems.ForEach(x => TimeGoals.Add(x));

            RegeistMessenger<double>(OnRequestFee);
        }

        void OnRequestFee(double fee)
        {
            if (fee == _defaultFee)
            {
                TimeGoal = TimeGoals.FirstOrDefault(x => x.Key == fee);
                if (TimeGoal == null)
                    TimeGoal = TimeGoals.FirstOrDefault();
            }
            else if (fee == _otherFee)
            {
                OtherChecked = true;
            }
            else
            {
                TradeFee = fee;
                CustomerChecked = true;
            }
        }

        public override void OnOkClick()
        {
            double fee = 0;
            if (CustomerChecked)
            {
                if (TradeFee <= 0)
                {
                    ShowMessage(LanguageService.Default.GetLanguageValue("Error_Fee"));
                    return;
                }
                fee = TradeFee;
            }
            else if (OtherChecked)
            {
                fee = _otherFee;
            }
            else
            {
                fee = _defaultFee;
            }
            base.OnOkClick();
            SendMessenger(Pages.SendPage, fee);
        }

        protected override string GetPageName()
        {
            return Pages.FeesPage;
        }

        private TimeGoalItem _timeGoal;

        public TimeGoalItem TimeGoal
        {
            get { return _timeGoal; }
            set
            {
                _timeGoal = value; RaisePropertyChanged("TimeGoal");
                OtherChecked = false;
                CustomerChecked = false;
            }
        }

        private bool _otherChecked = false;

        public bool OtherChecked
        {
            get { return _otherChecked; }
            set { _otherChecked = value; RaisePropertyChanged("OtherChecked"); }
        }

        private bool _customerChecked = true;

        public bool CustomerChecked
        {
            get { return _customerChecked; }
            set { _customerChecked = value; RaisePropertyChanged("CustomerChecked"); }
        }


        private bool _recommendChecked = true;

        public bool RecommendChecked
        {
            get { return _recommendChecked; }
            set { _recommendChecked = value; RaisePropertyChanged("RecommendChecked"); }
        }
    }
}