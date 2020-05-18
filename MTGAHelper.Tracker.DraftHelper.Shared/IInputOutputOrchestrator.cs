using MTGAHelper.Tracker.DraftHelper.Shared.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.DraftHelper.Shared
{
    public interface IInputOutputOrchestrator
    {
        ICollection<OutputModel> ProcessInput(InputModel inputModel, Bitmap screenshot);
    }
}
