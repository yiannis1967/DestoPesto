using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DestoPesto.Views
{
    public class SubmisionsListPageFlyoutMenuItem
    {
        public SubmisionsListPageFlyoutMenuItem()
        {
            TargetType = typeof(SubmisionsListPageFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}