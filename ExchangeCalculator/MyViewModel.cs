using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeCalculator
{
    public class MyViewModel : INotifyPropertyChanged
    {
        #region props
        public List<Currency> Currencies { get; set; }
        public string InputCurrencyId
        {
            get
            {
                return _inputCurrencyId;
            }
            set
            {
                if (string.CompareOrdinal(value, _inputCurrencyId) == 0) return;
                _inputCurrencyId = value;
                Compute();
            }
        }
        private string _inputCurrencyId;

        public double InputValue
        {
            get { return _inputValue; }
            set
            {
                if (Math.Abs(value - _inputValue) < double.Epsilon) return;
                _inputValue = value;
                RaisePropertyChanged("InputValue");
                Compute();
            }
        }
        private double _inputValue;

        public string OutputCurrencyId
        {
            get { return _outputConcurencyId; }
            set
            {
                if (string.CompareOrdinal(value, _outputConcurencyId) == 0) return;
                _outputConcurencyId = value;
                RaisePropertyChanged("OutputCurrencyId");
            }
        }
        private string _outputConcurencyId;

        public double OutputValue
        {
            get
            {
                return _outputValue;
            }
            set
            {
                if (Math.Abs(value - _outputValue) < double.Epsilon) return;
                _outputValue = value;
                RaisePropertyChanged("OutputValue");
            }
        }
        private double _outputValue;

        public bool IsValidInput
        {
            get
            {
                return !double.IsNaN(InputValue) &&
                Currencies.Exists(i => i.id == InputCurrencyId) &&
                Currencies.Exists(i => i.id == OutputCurrencyId);
            }
        }
        #endregion

        #region ctor

        public MyViewModel()
        {
            var currencies = Rates.GetBnrExchangeRates().ToList();
            Currencies = new List<Currency>();
            InputValue = 1.0;
            Currencies.Add(new Currency() { id = Guid.NewGuid().ToString(), currency = "RON", rate = 1 });
            Currencies.AddRange(currencies);
            InputCurrencyId = Currencies.Exists(i => string.Compare(i.currency, "eur", StringComparison.OrdinalIgnoreCase) == 0)
                ? Currencies.AsParallel().First(i => string.Compare(i.currency, "eur", StringComparison.OrdinalIgnoreCase) == 0).id
                : Currencies.First().id;
            OutputCurrencyId =
                Currencies.AsParallel().First(i => string.Compare(i.currency, "ron", StringComparison.OrdinalIgnoreCase) == 0).id;
            Compute();
        }
        #endregion

        private void Compute()
        {
            if (!IsValidInput) return;
            OutputValue = InputValue*Currencies.AsParallel().First(i => i.id == InputCurrencyId).rate/
                            Currencies.AsParallel().First(i => i.id == OutputCurrencyId).rate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}

