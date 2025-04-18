using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClubManagementSystem.Controllers.Common
{
    public class Week
    {
        public List<SelectListItem> GetWeekDropDown(int year)
        {
            List<SelectListItem> weeks = new List<SelectListItem>();

            for (int week = 1; week <= 52; week++)
            {
                var range = GetWeekRange(year, week);
                weeks.Add(new SelectListItem
                {
                    Value = week.ToString(),
                    Text = $"{range.StartOfWeek:MMM dd} - {range.EndOfWeek:MMM dd}"
                });
            }

            return weeks;
        }

        public (DateTime StartOfWeek, DateTime EndOfWeek) GetWeekRange(int year, int weekNumber)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)DayOfWeek.Monday - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset < 0 ? daysOffset + 7 : daysOffset);
            DateTime startOfWeek = firstMonday.AddDays((weekNumber - 1) * 7);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            return (startOfWeek, endOfWeek);
        }

        public (DateTime StartOfWeek, DateTime EndOfWeek) GetWeekRange(DateTime date)
        {
            int offset = (date.DayOfWeek == DayOfWeek.Sunday ? -6 : (int)DayOfWeek.Monday - (int)date.DayOfWeek);
            DateTime startOfWeek = date.AddDays(offset).Date;
            DateTime endOfWeek = startOfWeek.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (startOfWeek, endOfWeek);
        }

        public List<String> GetWeekDays(DateTime start)
        {
            var startDate = DateOnly.FromDateTime(start);
            var list = new List<String>();
            for (int i = 1; i < 7; i++)
            {
                list.Add($"{(DayOfWeek)i}<br>{startDate}");
                startDate = startDate.AddDays(1);
            }
            list.Add($"{(DayOfWeek)0}<br>{startDate}");
            return list;
        }
    }
}
