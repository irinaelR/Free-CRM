﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.DatabaseCleaningManager
{
    public interface IDatabaseCleanerService
    {
        Task RecreateDatabase(bool includeDemoData);
        void RepopulateWithDemo();
    }
}
