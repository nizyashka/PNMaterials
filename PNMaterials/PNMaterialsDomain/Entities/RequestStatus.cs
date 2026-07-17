using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PNMaterialsDomain.Entities
{
    public enum RequestStatus
    {
        [Description("СЗДН")] Created = 1,   // Создана, не передана в снабжение
        [Description("ВРБТ")] InWork = 2,    // Запущена в работу
        [Description("ВПЛН")] Completed = 3, // Закупка осуществлена
        [Description("УДЛН")] Deleted = 4    // Удалена
    }
}
