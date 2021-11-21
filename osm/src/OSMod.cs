using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.API.Datastructures;

namespace osm.src
{
    class OSMod : ModSystem
    {
        public override bool ShouldLoad(EnumAppSide side)
        {
            return side == EnumAppSide.Server;


        }
        ICoreServerAPI serverApi;
        ICoreServerAPI capi;
        List<string> SzukamLista;
        List<string> OgłoszenieDodaj;
        List<string> OgłoszenieDodajOgólne;

        public List<string> OgłoszeniaDodaj { get; private set; }
        public List<string> OgłoszeniaDodajOgólne { get; private set; }

        public override void StartServerSide(ICoreServerAPI api)
        {
            serverApi = api;
            this.capi = api;


            api.Event.GameWorldSave += OnSaveGameSaving;
            api.Event.SaveGameLoaded += OnSaveGameLoading;
            

            api.RegisterCommand("szukam", "Jeżeli szukasz kogoś do wspólnej gry możesz zapisać się do listy.", "[lista|dołącz|opuść|test]", OnSzukam);
            api.RegisterCommand("regulamin", "Regulamin serwera", "[frakcje|pvp|pve|ogólny]", OnRegulamin);
            api.RegisterCommand("ogłoszenia", "Ogłoszenia serwera", "[dodaj|dodaj_ogólne|usuń|usuń_ogólne|handel|ogólne]", OnOgłoszenia);
            api.RegisterCommand("komendy", "Przydatne komendy", "", OnKomendy);
            



            Console.WriteLine("[ServerOzaruMod] Mod aktywowany, regulamin załadowany");
        }

        


        

        private void OnKomendy(IServerPlayer player, int groupId, CmdArgs args)
        {
            string komendy = "Przydatne komendy:\n/spawn - Teleportuje gracza na spawn.\n/szukam - Zapisuje gracza do listy osób, którzy szukają osób do wspólnej gry czy frakcji.\n/ogłoszenia- Dodaje możliwośc dodania własnych ogłoszeń.\n/regulamin- Regulamin serwera.\n/home- Teleportuje gracza do wcześniej ustawionego domu.\n/sethome- Ustawienie domu.\n/players- Aktualna lista osób online.\n/land claim new- Zabezpieczenie terenu.\n/land claim free [id]- Usunięcie claima.";

            player.SendMessage(groupId, komendy, EnumChatType.CommandError);
        }
        private void OnOgłoszenia(IServerPlayer player, int groupId, CmdArgs args)
        {
            string cmd1 = args.PopWord();
            switch (cmd1)
            {

                case "dodaj":
                    {

                        if (args.Length == 0)
                        {
                            player.SendMessage(groupId, "/ogłoszenia dodaj [treść]", EnumChatType.CommandError);
                        }
                        else
                        {
                            DateTime now = DateTime.Now;

                            OgłoszenieDodaj.Add("<font color= '#5B5FA6'><strong>" + player.PlayerName + "</strong></font>" + "<font color= '#5B5FA6' align= 'right'><strong>" + now.ToString("dd.MM.yyyy, H:mm:ss") + "</strong></font>");
                            OgłoszenieDodaj.Add("<font color= '#F2B705' align= 'center'><i>" + args.PopAll() + "</i></font>");
                            OgłoszenieDodaj.Add("<font color= '#F2F2F2' align= 'center'>___________________________________________________________________________</font>");

                            string linia = "<font color= '#F2F2F2' align= 'center'>___________________________________________________________________________</font>";
                            string wiadomośćReszta = "<font color= '#F2F2F2' align= 'center'><strong> Dodał/a nowe ogłoszenie handlowe!</strong ></font> ";
                            string wiadomośćGracz = "<font color= '#5B5FA6' align= 'center'><strong><i>" + player.PlayerName + "</i></strong ></font>";

                            player.SendMessage(groupId, "Dodałeś ogłoszenie !", EnumChatType.CommandSuccess);
                            this.serverApi.BroadcastMessageToAllGroups(linia + "\n" + wiadomośćGracz + "\n" + wiadomośćReszta + linia, EnumChatType.OthersMessage);

                        }


                    }
                    break;

                case "dodaj_ogólne":
                    {
                        if (player.HasPrivilege("OgłoszeniaDodajOgólne"))
                        {
                            if (args.Length == 0)
                            {
                                player.SendMessage(groupId, "/ogłoszenia dodaj_ogólne [treść]", EnumChatType.CommandError);
                            }
                            else
                            {
                                DateTime now = DateTime.Now;

                                OgłoszenieDodajOgólne.Add("<font color= '#D94425'><strong>" + player.PlayerName + "</strong></font>" + "<font color= '#5B5FA6' align= 'right'><strong>" + now.ToString("dd.MM.yyyy, H:mm:ss") + "</strong></font>");
                                OgłoszenieDodajOgólne.Add("<font color= '#F2B705' align= 'center'><i>" + args.PopAll() + "</i></font>");
                                OgłoszenieDodajOgólne.Add("<font color= '#F2F2F2' align= 'center'>___________________________________________________________________________</font>");

                                string linia = "<font color= '#F2F2F2' align= 'center'>___________________________________________________________________________</font>";
                                string wiadomośćReszta = "<font color= '#F2F2F2' align= 'center'><strong> Dodał nowe ogłoszenie !</strong ></font> ";
                                string wiadomośćGracz = "<font color= '#D94425' align= 'center'><strong><i>" + player.PlayerName + "</i></strong ></font>";

                                player.SendMessage(groupId, "Dodałeś ogłoszenie !", EnumChatType.CommandSuccess);
                                this.serverApi.BroadcastMessageToAllGroups(linia + "\n" + wiadomośćGracz + "\n" + wiadomośćReszta + linia, EnumChatType.OthersMessage);


                            }
                        }
                        else
                        {
                            player.SendMessage(groupId, "<font color= '#BD3939'>\nNie masz wystarczających uprawnień, aby użyc tej komendy !</font>", EnumChatType.CommandSuccess);
                        }
                    }
                    break;

                case "usuń":
                    {
                        if (player.HasPrivilege("OgłoszeniaUsuń"))
                        {
                            OgłoszenieDodaj.Clear();
                            player.SendMessage(groupId, "Usunąłeś ogłoszenie !", EnumChatType.CommandSuccess);
                        }
                        else
                        {
                            player.SendMessage(groupId, "<font color= '#BD3939'>\nNie masz wystarczających uprawnień, aby użyc tej komendy !</font>", EnumChatType.CommandSuccess);
                        }
                    }
                    break;

                case "usuń_ogólne":
                    {
                        if (player.HasPrivilege("OgłoszeniaUsuńOgólne"))
                        {
                            OgłoszenieDodajOgólne.Clear();
                            player.SendMessage(groupId, "Usunąłeś ogłoszenia !", EnumChatType.CommandSuccess);
                        }
                        else
                        {
                            player.SendMessage(groupId, "<font color= '#BD3939'>\nNie masz wystarczających uprawnień, aby użyc tej komendy !</font>", EnumChatType.CommandSuccess);
                        }
                    }
                    break;

                case "handel":
                    {
                        if (OgłoszenieDodaj.Count == 0)
                        {
                            player.SendMessage(groupId, "\n<font color= '#F29F05' align= 'center'><strong>OGŁOSZENIA HANDLOWE</strong></font>\n", EnumChatType.CommandError);
                        }
                        else
                        {
                            string OgłoszeniaHandel = "\n<font color= '#F29F05' align= 'center'><strong>OGŁOSZENIA HANDLOWE</strong></font>\n";

                            player.SendMessage(groupId, OgłoszeniaHandel, EnumChatType.CommandSuccess);

                            foreach (String OgłoszeniaDodaj in OgłoszenieDodaj)
                            {
                                player.SendMessage(groupId, OgłoszeniaDodaj, EnumChatType.CommandSuccess);
                            }


                        }

                    }
                    break;


                case "ogólne":
                    {
                        if (OgłoszenieDodajOgólne.Count == 0)
                        {
                            player.SendMessage(groupId, "\n<font color= '#F29F05' align= 'center'><strong>OGŁOSZENIA</strong></font>\n", EnumChatType.CommandError);
                        }
                        else
                        {
                            string OgłoszeniaOgólny = "\n<font color= '#F29F05' align= 'center'><strong>OGŁOSZENIA</strong></font>\n";

                            player.SendMessage(groupId, OgłoszeniaOgólny, EnumChatType.CommandSuccess);

                            foreach (String OgłoszeniaDodajOgólne in OgłoszenieDodajOgólne)
                            {
                                player.SendMessage(groupId, OgłoszeniaDodajOgólne, EnumChatType.CommandSuccess);

                            }


                        }
                    }
                    break;

                case null:
                default:
                    player.SendMessage(groupId, "/ogłoszenia [dodaj|dodaj_ogólne|usuń|usuń_ogólne|handel|ogólne]", EnumChatType.CommandError);
                    return;
            }
        }

        private void OnRegulamin(IServerPlayer player, int groupId, CmdArgs args)
        {
            string cmd = args.PopWord();
            switch (cmd)
            {
                case "frakcje":
                    {
                        player.SendMessage(groupId, "<font align= 'center'>REGULAMIN FRAKCJI</font>\n\n1.1 Aby dana grupa mogła nazwać się frakcją muszą zostać spełnione odpowiednie warunki.\n\t1.1.1 Posiadać minimalną ilość członków w ilości trzech.\n\t1.1.2 Musi zostać wyznaczony przywódca grupy.\n\t1.1.3 Posiadać minimalną ilość budynków zaakceptowaną przez administrację typu siedziba frakcji, droga z bazy na spawn, minimalnie jeden budynek mieszkalny, które muszą wykonać w ciągu tygodnia od zgłoszenia frakcji.\n1.2 Frakcje mogą wypowiadać między sobą wojny, jeżeli posiadają ku temu dobry powód.", EnumChatType.CommandSuccess);
                    }
                    break;


                case "pvp":
                    {
                        player.SendMessage(groupId, "<font align= 'center'>REGULAMIN STREFY PvP</font>\n\n1.1 Granica PvP zaczyna się 2500 kratek od spawnu(współrzędna X lub Y więcej niż 2500, lub współrzędna X lub Y mniejsza niż 2500).\n1.2 PvP jest dozwolone bez żadnych konsekwencji.\n1.3 Administracja nie odpowiada za utracone przedmioty.\n1.4 Gracz może osiedlić się w strefie PvP lecz musi liczyć się z konsekwencjami swojego wyboru.\n1.5 Posiadanie claimu w strefie PvP wymaga zgody administracji.", EnumChatType.CommandSuccess);
                    }
                    break;


                case "pve":
                    {
                        player.SendMessage(groupId, "<font align= 'center'>REGULAMIN STREFY OGRANICZONE PvP</font>\n\n1.1 Wszelkie ataki na graczy są zakazane za wyjątkiem zgłoszenia oficjalnej wojny między frakcjami administracji.\n1.2 W konflikty może interweniować administracja jeśli uzna, że jedna strona stosuje nieuczciwe lub niezgodne z regulaminem metody np.nękanie graczy, griefing itp,\n1.3 Zakaz okradania graczy nieobecnych na serwerze.", EnumChatType.CommandSuccess);
                    }
                    break;


                case "ogólny":
                    {
                        string regulamin = "<font align= 'center'>REGULAMIN SERWERA OZARU.PL</font>\n\n1.1 Nieprzestrzeganie regulaminu wiąże się z otrzymaniem kary.\n1.2 Nieznajomość regulaminu nie zwalnia z jego przestrzegania.\n1.3 Administracja ma pełne prawa do zmieniania treści regulaminu bez wcześniejszego powiadomienia użytkowników o zmianie.\n1.4 Zakaz nękania graczy.\n1.5 Zakaz griefingu.";

                        player.SendMessage(groupId, regulamin, EnumChatType.CommandSuccess);
                    }
                    break;

                case null:
                default:
                    player.SendMessage(groupId, "/regulamin [frakcje|pvp|pve|ogólny]", EnumChatType.CommandError);
                    return;


            }
        }

        private void OnSzukam(IServerPlayer player, int groupId, CmdArgs args)
        {
            string cmd = args.PopWord();
            switch (cmd)
            {
                case "lista":
                    if (SzukamLista.Count == 0)
                    {
                        player.SendMessage(groupId, "<strong><i>Nikt nie szuka frakcji</i></strong>", EnumChatType.CommandSuccess);
                    }
                    else
                    {
                        string odpowiedź = "<font color= '#9FA66A'>Gracze szukający frakcji:</font>";
                        SzukamLista.ForEach((playerUid) =>
                        {
                            odpowiedź += "\n" + "\t<font color= '#8FA65D'><icon name=hat></font></icon><font color= '#5D733C'><strong> " + serverApi.World.PlayerByUid(playerUid).PlayerName + "</strong></font>";
                        });

                        player.SendMessage(groupId, odpowiedź, EnumChatType.CommandSuccess);
                    }
                    break;

                case "dołącz":
                    if (SzukamLista.Contains(player.PlayerUID))
                    {
                        player.SendMessage(groupId, "Zostałeś już zapisany!", EnumChatType.CommandError);
                    }
                    else
                    {
                        SzukamLista.Add(player.PlayerUID);

                        player.SendMessage(groupId, "Zostałeś zapisany.", EnumChatType.CommandSuccess);

                        string linia = "<font color= '#F2F2F2' align= 'center'>___________________________________________________________________________</font>";
                        string wiadomośćReszta = "<font color= '#F2F2F2' align= 'center'><strong> Szukam osób do wspólnej gry !</strong ></font> ";
                        string wiadomośćGracz = "<font color= '#D94425' align= 'center'><strong><i>" + player.PlayerName + "</i></strong ></font>";

                        this.serverApi.BroadcastMessageToAllGroups(linia + "\n" + wiadomośćGracz + "\n" + wiadomośćReszta + linia, EnumChatType.OthersMessage);
                    }
                    break;

                case "opuść":
                    if (!SzukamLista.Remove(player.PlayerUID))
                    {
                        player.SendMessage(groupId, "Nie ma cię na liście.", EnumChatType.CommandError);
                    }
                    else
                    {
                        player.SendMessage(groupId, "Zostałeś usunięty z listy", EnumChatType.CommandSuccess);
                    }
                    break;

                case "test":
                    if (player.HasPrivilege("testowa"))
                    {
                        player.SendMessage(groupId, "Masz permisje", EnumChatType.CommandError);
                    }
                    else
                    {
                        player.SendMessage(groupId, "Nie Masz permisji", EnumChatType.CommandSuccess);
                    }
                    break;

                case null:
                default:
                    player.SendMessage(groupId, "/szukam [lista|dołącz|opuść]", EnumChatType.CommandError);
                    return;
            }
        }

        private void OnSaveGameLoading()
        {
            byte[] data = this.serverApi.WorldManager.SaveGame.GetData("szukam");
            this.serverApi.WorldManager.SaveGame.GetData("ogłoszenia");

            this.SzukamLista = data == null ? new List<string>() : SerializerUtil.Deserialize<List<string>>(data);
            this.OgłoszenieDodaj = data == null ? new List<string>() : SerializerUtil.Deserialize<List<string>>(data);
            this.OgłoszenieDodajOgólne = data == null ? new List<string>() : SerializerUtil.Deserialize<List<string>>(data);
        }
        private void OnSaveGameSaving()
        {
            serverApi.WorldManager.SaveGame.StoreData("szukam", SerializerUtil.Serialize(SzukamLista));
            serverApi.WorldManager.SaveGame.StoreData("ogłoszenia", SerializerUtil.Serialize(OgłoszenieDodaj));
            serverApi.WorldManager.SaveGame.StoreData("ogłoszenia", SerializerUtil.Serialize(OgłoszenieDodajOgólne));
        }

        
    }
}
