using System.Collections.Generic;

namespace Teller.Core.Repository
{
    /// <summary>
    /// Generisk interface for repository-funksjonalitet for å stappe entities i databasen, lese de ut osv.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Hent en instans av et objekt basert på id
        /// </summary>
        /// <param name="id">Id på objektet som skal hentes</param>
        /// <returns>Objektet, eller null hvis ikke noe objekt med angitt id eksisterer</returns>
        T Get(long id);

        /// <summary>
        /// Hent alle objekter av denne typen fra databasen
        /// </summary>
        /// <returns>En IEnumerable med alle objektene som finnes i databasen</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Lagre et objekt i databasen
        /// </summary>
        /// <param name="evt">Objektet som skal lagres</param>
        void Store(T evt);

        /// <summary>
        /// Lagre endringer på et objekt i databasen
        /// </summary>
        /// <param name="evt">Objektet som skal oppdateres</param>
        void Update(T evt);

        /// <summary>
        /// Slette et objekt fra databasen
        /// </summary>
        /// <param name="id">Unik id på objektet som skal slettes</param>
        void Delete(long id);
    }
}
