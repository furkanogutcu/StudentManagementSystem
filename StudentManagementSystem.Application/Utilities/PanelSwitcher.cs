using System.Collections.Generic;
using System.Windows.Forms;

namespace StudentManagementSystem.Application.Utilities
{
    public static class PanelSwitcher
    {
        public static void ShowPanel(Panel panel, List<Panel> panelList)
        {
            panel.Visible = true;
            HideOtherPanels(panel, panelList);
        }

        public static void HideOtherPanels(Panel panelToShow, List<Panel> panelList)
        {
            foreach (var panel in panelList.FindAll(p => p.Name != panelToShow.Name))
            {
                panel.Visible = false;
                PanelCleaner.Clear(panel);
            }
        }
    }
}
