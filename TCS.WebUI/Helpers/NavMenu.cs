using System.Reflection;
using System.Resources;
using TCS.WebUI.Models;

namespace TCS.WebUI.Helpers
{
    public static class NavMenu
    {
        static ResourceManager rm = new ResourceManager("TCS.WebUI.Resources.SharedResource", Assembly.GetExecutingAssembly());

        public static SmartNavigation NavMenuLoad()
        {
            //smartItemSub.Roles = new string[] { "Home-Admin" };
            SmartNavigation smart = new SmartNavigation();

            smart.Lists.Add(SmartInicio());
            smart.Lists.Add(SmartUsuario());
            smart.Lists.Add(SmartAlert());
            smart.Lists.Add(SmartDesign());
            smart.Lists.Add(SmartDevTools());
            smart.Lists.Add(SmartInspeccion());
            smart.Lists.Add(SmartTelares());
            smart.Lists.Add(SmartInventario());
            smart.Lists.Add(SmartHollandSherry());
            smart.Lists.Add(SmartEnglishAmerican());
            smart.Lists.Add(SmartMeeting());
            smart.Lists.Add(SmartEvDesempeño());
            smart.Version = "4.5";

            return smart;
        }

        public static ListItem SmartInicio()
        {
            ListItem smartInicio = new ListItem();
            smartInicio.Icon = "fal fa-home";
            smartInicio.Title = rm.GetString("home");
            //smartInicio.Title = "Inicio";
            smartInicio.Type = ItemType.Single;

            ListItem smartItemIndex = new ListItem();
            smartItemIndex.Href = "home_index.html";
            smartItemIndex.Title = rm.GetString("dashboard");
            smartItemIndex.Type = ItemType.Single;
            smartInicio.Items.Add(smartItemIndex);

            ListItem smartItemDirectorio = new ListItem();
            smartItemDirectorio.Href = "home_directory.html";
            smartItemDirectorio.Title = rm.GetString("directory");
            smartItemDirectorio.Type = ItemType.Single;
            smartInicio.Items.Add(smartItemDirectorio);

            return smartInicio;
        }

        public static ListItem SmartUsuario()
        {
            ListItem smartUsuario = new ListItem();
            smartUsuario.Icon = "fal fa-table";
            smartUsuario.Title = rm.GetString("users");
            smartUsuario.Type = ItemType.Single;
            smartUsuario.Roles = new string[] { "Users-Admin", "Users-Private", "Users-Public"};

            ListItem smartItemAsignacionPermiso = new ListItem();
            smartItemAsignacionPermiso.Href = "permissionassignment_index.html";
            smartItemAsignacionPermiso.Title = rm.GetString("permissionassignment");
            smartItemAsignacionPermiso.Type = ItemType.Single;
            smartItemAsignacionPermiso.Roles = new string[] { "Permission Assignment-Admin", "Permission Assignment-Private", "Permission Assignment-Public" };
            smartUsuario.Items.Add(smartItemAsignacionPermiso);

            ListItem smartItemUsuario = new ListItem();
            smartItemUsuario.Href = "users_index.html";
            smartItemUsuario.Title = rm.GetString("users");
            smartItemUsuario.Type = ItemType.Single;
            smartItemUsuario.Roles = new string[] { "Users-Admin", "Users-Private", "Users-Public" };
            smartUsuario.Items.Add(smartItemUsuario);

            ListItem smartItemModulo = new ListItem();
            smartItemModulo.Href = "modules_index.html";
            smartItemModulo.Title = rm.GetString("modules");
            smartItemModulo.Type = ItemType.Single;
            smartItemUsuario.Roles = new string[] { "Modules-Admin", "Modules-Private", "Modules-Public" };
            smartUsuario.Items.Add(smartItemModulo);

            ListItem smartItemPermiso = new ListItem();
            smartItemPermiso.Href = "permissions_index.html";
            smartItemPermiso.Title = rm.GetString("permissions");
            smartItemPermiso.Type = ItemType.Single;
            smartItemUsuario.Roles = new string[] { "Permissions-Admin", "Permissions-Private", "Permissions-Public" };
            smartUsuario.Items.Add(smartItemPermiso);

            return smartUsuario;
        }

        public static ListItem SmartAlert()
        {
            ListItem SmartAlert = new ListItem();
            SmartAlert.Icon = "fal fa-table";
            SmartAlert.Title = rm.GetString("alert");
            SmartAlert.Type = ItemType.Single;
            SmartAlert.Roles = new string[] { "Alert-Admin", "Alert-Private", "Alert-Public" };

            ListItem smartPermisoAlerta = new ListItem();
            smartPermisoAlerta.Href = "alert_alertpermission.html";
            smartPermisoAlerta.Title = rm.GetString("alertPermission");
            smartPermisoAlerta.Type = ItemType.Single;
            smartPermisoAlerta.Roles = new string[] { "Alert-Admin", "Alert-Private", "Alert-Public" };
            SmartAlert.Items.Add(smartPermisoAlerta);

            return SmartAlert;
        }

        public static ListItem SmartDesign()
        {
            ListItem smartDesign = new ListItem();
            smartDesign.Icon = "fal fa-table";
            smartDesign.Title = rm.GetString("design");
            smartDesign.Type = ItemType.Single;
            smartDesign.Roles = new string[] { "Design-Admin", "Design-Private", "Design-Public"};

            ListItem smartItemDraft = new ListItem();
            smartItemDraft.Href = "pegplan_index.html";
            smartItemDraft.Title = "PegPlan";
            smartItemDraft.Type = ItemType.Single;
            smartDesign.Items.Add(smartItemDraft);

            ListItem smartItemSley = new ListItem();
            smartItemSley.Href = "draft_index.html";
            smartItemSley.Title = "Draft";
            smartItemSley.Type = ItemType.Single;
            smartDesign.Items.Add(smartItemSley);


            ListItem smartItemDraftSley = new ListItem();
            smartItemDraftSley.Href = "pegplandraft_index.html";
            smartItemDraftSley.Title = "PegPlanDraft";
            smartItemDraftSley.Type = ItemType.Single;
            smartDesign.Items.Add(smartItemDraftSley);

            ListItem smartItemDesign = new ListItem();
            smartItemDesign.Href = "design_index.html";
            smartItemDesign.Title = "Design";
            smartItemDesign.Type = ItemType.Single;
            smartDesign.Items.Add(smartItemDesign);
            return smartDesign;
        }

        public static ListItem SmartDevTools()
        {
            ListItem smartDesign = new ListItem();
            smartDesign.Icon = "fal fa-table";
            smartDesign.Title = "DevTools";
            smartDesign.Type = ItemType.Single;
            smartDesign.Roles = new string[] { "DevTools-Admin", "DevTools-Private", "DevTools-Public" };

            ListItem smartItemDraft = new ListItem();
            smartItemDraft.Href = "projects_index.html";
            smartItemDraft.Title = "Projects";
            smartItemDraft.Type = ItemType.Single;
            smartDesign.Items.Add(smartItemDraft);

            return smartDesign;
        }

        public static ListItem SmartInspeccion()
        {
            ListItem smartInspection = new ListItem();
            smartInspection.Icon = "fal fa-table";
            smartInspection.Title = rm.GetString("inspection");
            smartInspection.Type = ItemType.Single;
            smartInspection.Roles = new string[] { "Inspection-Admin", "Inspection-Private", "Inspection-Public" };

            ListItem smartInspectPiece = new ListItem();
            smartInspectPiece.Href = "inspection_inspectpiece.html";
            smartInspectPiece.Title = rm.GetString("inspectpiece");
            smartInspectPiece.Type = ItemType.Single;
            smartInspection.Items.Add(smartInspectPiece);

            ListItem smartRetainedPiece = new ListItem();
            smartRetainedPiece.Href = "inspection_retainedpiece.html";
            smartRetainedPiece.Title = rm.GetString("retainedpiece");
            smartRetainedPiece.Type = ItemType.Single;
            smartInspection.Items.Add(smartRetainedPiece);

            ListItem smartUnshippedPiece = new ListItem();
            smartUnshippedPiece.Href = "inspection_unshippedpiece.html";
            smartUnshippedPiece.Title = rm.GetString("unshippedpiece");
            smartUnshippedPiece.Type = ItemType.Single;
            smartInspection.Items.Add(smartUnshippedPiece);

            ListItem smartBinnacle = new ListItem();
            smartBinnacle.Href = "inspection_binnacle.html";
            smartBinnacle.Title = rm.GetString("binnacle");
            smartBinnacle.Type = ItemType.Single;
            smartInspection.Items.Add(smartBinnacle);

            ListItem smartNewPiece = new ListItem();
            smartNewPiece.Href = "inspection_newpiece.html";
            smartNewPiece.Title = rm.GetString("newpiece");
            smartNewPiece.Type = ItemType.Single;
            smartInspection.Items.Add(smartNewPiece);

            ListItem smartReports = new ListItem();
            smartReports.Href = "inspection_reports.html";
            smartReports.Title = rm.GetString("reports");
            smartReports.Type = ItemType.Single;
            smartInspection.Items.Add(smartReports);

            return smartInspection;
        }

        public static ListItem SmartTelares()
        {
            ListItem smartTelares = new ListItem();
            smartTelares.Icon = "fal fa-table";
            smartTelares.Title = rm.GetString("weaving");
            smartTelares.Type = ItemType.Single;
            smartTelares.Roles = new string[] { "Weaving-Admin", "Weaving-Private", "Weaving-Public"};

            ListItem smartItemWeavingIndex = new ListItem();
            smartItemWeavingIndex.Href = "weaving_index.html";
            smartItemWeavingIndex.Title = rm.GetString("dashboard");
            smartItemWeavingIndex.Type = ItemType.Single;
            smartItemWeavingIndex.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemWeavingIndex);

            ListItem smartItemTurnoActual = new ListItem();
            smartItemTurnoActual.Href = "weaving_loomlist.html";
            smartItemTurnoActual.Title = rm.GetString("currentshift");
            smartItemTurnoActual.Type = ItemType.Single;
            smartItemTurnoActual.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemTurnoActual);

            ListItem smartItemProduccionDiaria = new ListItem();
            smartItemProduccionDiaria.Href = "weaving_dailyproduction.html";
            smartItemProduccionDiaria.Title = rm.GetString("dailyproduction");
            smartItemProduccionDiaria.Type = ItemType.Single;
            smartItemProduccionDiaria.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemProduccionDiaria);

            ListItem smartItemProduccionTurno = new ListItem();
            smartItemProduccionTurno.Href = "weaving_shiftproduction.html";
            smartItemProduccionTurno.Title = rm.GetString("shiftproduction");
            smartItemProduccionTurno.Type = ItemType.Single;
            smartItemProduccionTurno.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemProduccionTurno);

            ListItem smartItemSumarioProduccion = new ListItem();
            smartItemSumarioProduccion.Href = "weaving_generalproduction.html";
            smartItemSumarioProduccion.Title = rm.GetString("productionsummary");
            smartItemSumarioProduccion.Type = ItemType.Single;
            smartItemSumarioProduccion.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemSumarioProduccion);

            ListItem smartItemAsignacionTurnos = new ListItem();
            smartItemAsignacionTurnos.Href = "weaving_weekassignation.html";
            smartItemAsignacionTurnos.Title = rm.GetString("shiftsassignment");
            smartItemAsignacionTurnos.Type = ItemType.Single;
            smartItemAsignacionTurnos.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemAsignacionTurnos);

            ListItem smartItemWorkers = new ListItem();
            smartItemWorkers.Href = "workers_index.html";
            smartItemWorkers.Title = rm.GetString("workers");
            smartItemWorkers.Type = ItemType.Single;
            smartItemWorkers.Roles = new string[] { "Workers-Admin", "All" };
            smartTelares.Items.Add(smartItemWorkers);

            ListItem smartItemParosTurno = new ListItem();
            smartItemParosTurno.Href = "weaving_shiftstop.html";
            smartItemParosTurno.Title = rm.GetString("stoppagecurrentshift");
            smartItemParosTurno.Type = ItemType.Single;
            smartItemParosTurno.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemParosTurno);

            ListItem smartItemHistorialParos = new ListItem();
            smartItemHistorialParos.Href = "weaving_stopproductionhistory.html";
            smartItemHistorialParos.Title = rm.GetString("stoppagehistory");
            smartItemHistorialParos.Type = ItemType.Single;
            smartItemHistorialParos.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemHistorialParos);

            ListItem smartItemDetalleParo = new ListItem();
            smartItemDetalleParo.Href = "weaving_stopproductionhistorydetail.html";
            smartItemDetalleParo.Title = rm.GetString("stopdetail");
            smartItemDetalleParo.Type = ItemType.Single;
            smartItemDetalleParo.Roles = new string[] { "Weaving-Admin", "All" };
            smartTelares.Items.Add(smartItemDetalleParo);

            ListItem smartItemTelaresTerminar = new ListItem();
            smartItemTelaresTerminar.Href = "weaving_penultimatepiece.html";
            smartItemTelaresTerminar.Title = rm.GetString("loomtofinish");
            smartItemTelaresTerminar.Roles = new string[] { "Weaving-Admin", "All" };
            smartItemTelaresTerminar.Type = ItemType.Single;
            smartTelares.Items.Add(smartItemTelaresTerminar);

            return smartTelares;
        }

        public static ListItem SmartInventario()
        {
            ListItem smartInicio = new ListItem();
            smartInicio.Icon = "fal fa-table";
            smartInicio.Title = rm.GetString("inventory");
            smartInicio.Type = ItemType.Single;
            smartInicio.Roles = new string[] { "Inventory-Admin", "Inventory-Private", "Inventory-Public" };

            ListItem smartItemIndex = new ListItem();
            smartItemIndex.Href = "inventory_index.html";
            smartItemIndex.Title = rm.GetString("dashboard");
            smartItemIndex.Type = ItemType.Single;
            smartInicio.Items.Add(smartItemIndex);

            ListItem smartItemDirectorio = new ListItem();
            smartItemDirectorio.Href = "inventory_inventories.html";
            smartItemDirectorio.Title = rm.GetString("inventories");
            smartItemDirectorio.Type = ItemType.Single;
            smartInicio.Items.Add(smartItemDirectorio);

            ListItem smartItemSearch = new ListItem();
            smartItemSearch.Href = "inventory_piecesearcher.html";
            smartItemSearch.Title = rm.GetString("piecesearcher");
            smartItemSearch.Type = ItemType.Single;
            smartInicio.Items.Add(smartItemSearch);

            return smartInicio;
        }

        public static ListItem SmartHollandSherry()
        {
            ListItem smartInicio = new ListItem();
            smartInicio.Icon = "fal fa-table";
            smartInicio.Title = "Orders";
            smartInicio.Type = ItemType.Single;
            smartInicio.Roles = new string[] { "HollandSherry-Admin", "HollandSherry-Private", "HollandSherry-Public" };

            ListItem smartTracking = new ListItem();
            smartTracking.Href = "hollandsherry_order.html";
            smartTracking.Title = rm.GetString("tracking");
            smartTracking.Type = ItemType.Single;
            smartInicio.Items.Add(smartTracking);

            return smartInicio;
        }

        public static ListItem SmartEnglishAmerican()
        {
            ListItem smartInicio = new ListItem();
            smartInicio.Icon = "fal fa-table";
            smartInicio.Title = "Invoicing";
            smartInicio.Type = ItemType.Single;
            smartInicio.Roles = new string[] { "English American-Admin", "English American-Private", "English American-Public" };

            ListItem smartTracking = new ListItem();
            smartTracking.Href = "englishamerican_invoice.html";
            smartTracking.Title = rm.GetString("invoice");
            smartTracking.Type = ItemType.Single;
            smartInicio.Items.Add(smartTracking);

            return smartInicio;
        }

        public static ListItem SmartMeeting()
        {
            ListItem smartInicio = new ListItem();
            smartInicio.Icon = "fal fa-table";
            smartInicio.Title = rm.GetString("meeting");
            smartInicio.Type = ItemType.Single;
            smartInicio.Roles = new string[] { "Meeting-Admin", "Meeting-Private", "Meeting-Public" };

            ListItem smartMeeting = new ListItem();
            smartMeeting.Href = "meetings_index.html";
            smartMeeting.Title = rm.GetString("meeting");
            smartMeeting.Type = ItemType.Single;
            smartInicio.Items.Add(smartMeeting);

            return smartInicio;
        }

        public static ListItem SmartEvDesempeño()
        {
            ListItem smartEvDesempeño = new ListItem();
            smartEvDesempeño.Icon = "fal fa-edit";
            smartEvDesempeño.Title = rm.GetString("evaluation");
            smartEvDesempeño.Type = ItemType.Single;

            ListItem smartEvTrabajador = new ListItem();
            smartEvTrabajador.Href = "performanceevaluation_index.html";
            smartEvTrabajador.Title = rm.GetString("workerevaluation");
            smartEvTrabajador.Type = ItemType.Single;
            smartEvDesempeño.Items.Add(smartEvTrabajador);

            ListItem smartEvDepartamento = new ListItem();
            smartEvDepartamento.Href = "performanceevaluation_scoresdeptos.html";
            smartEvDepartamento.Title = rm.GetString("departmentevaluation");
            smartEvDepartamento.Type = ItemType.Single;
            smartEvDepartamento.Roles = new string[] { "Evaluation-Admin", "Evaluation-Private", "Evaluation-Public" };
            smartEvDesempeño.Items.Add(smartEvDepartamento);

            return smartEvDesempeño;
        }
    }
}
