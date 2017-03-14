using System.Web.Http;
using Teller.Core.Repository;

namespace StautApi.Controllers
{
    [RoutePrefix("api/measurements")]
    public class MeasurementsController : ApiController
    {
        #region Fields

        private readonly IMeasurementRepository _measurementRepository;

        #endregion

        public MeasurementsController(IMeasurementRepository measurementRepository)
        {
            _measurementRepository = measurementRepository;
        }
    }
}
