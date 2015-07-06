using System.Configuration;

namespace Teller.Core.Infrastructure
{
    /// <summary>
    /// En klasse som brukes for å lese ut konfigurasjonsinformasjon for STAut-systemet
    /// </summary>
    public class StautConfiguration
    {
        #region Fields

        private static StautConfiguration _currentConfiguration;

        #endregion

        #region Public properties

        /// <summary>
        /// Filområdet der data blir lagret - som vi så skal hente ut for å stappe det i databasen
        /// </summary>
        public string CollectorDirectory { get; set; }

        /// <summary>
        /// Filområdet som datafiler blir flyttet til når vi har lest dem inn i databasen
        /// </summary>
        public string StorageDirectory { get; set; }

        #endregion

        #region Static properties

        /// <summary>
        /// Gjeldende konfigurasjon
        /// </summary>
        public static StautConfiguration Current {
            get { return _currentConfiguration ?? (_currentConfiguration = GetDefaultConfiguration()); }
            set { _currentConfiguration = value; }
        }

        #endregion

        #region Private methods

        private static StautConfiguration GetDefaultConfiguration()
        {
            return new StautConfiguration
            {
                CollectorDirectory = ConfigurationManager.AppSettings["collectorDirectory"],
                StorageDirectory = ConfigurationManager.AppSettings["storageDirectory"]
            };
        }

        #endregion
    }
}
