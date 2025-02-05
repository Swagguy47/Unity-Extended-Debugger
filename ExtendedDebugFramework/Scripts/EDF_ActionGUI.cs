using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtendedDebugFramework
{
    //  creates a hierarchy of GUI buttons for commands
    public class EDF_ActionGUI : MonoBehaviour
    {
        [SerializeField] Transform ContentRoot;

        [SerializeField] EDF_GUIButton guiButton;
        [SerializeField] EDF_GUIInput stringInput, boolInput;
        [SerializeField] GameObject pageIcon, folderRoot;

        List<Transform> pages = new();

        public static EDF_ActionGUI self;

        //HACK
        WaitForSeconds runtimeDelay = new(0.01f);
        int pageCount;
        int iterations;

        void Start()
        {
            self = this;
            CreateGUI();
            EDF_Commands.commandsRegistered += RuntimeGUI;
        }

        //  --------------------------------
        //  Runtime Re-registering
        //  TODO: every other time CreateGUI() runs, it doesn't work,
        //  This implimentation a hack to get around the issue
        void RuntimeGUI()
        {
            iterations = 0;
            CreateGUI();
            //StartCoroutine(IRuntimeGUI());
        }

        IEnumerator IRuntimeGUI()
        {
            yield return runtimeDelay;
            CreateGUI();
        }
        //  --------------------------------

        void CreateGUI()
        {
            iterations++;
            if (iterations > 4)
                return;

            pages.Clear();
            ClearChildren();

            //  create root
            var rootPage = Instantiate(folderRoot, ContentRoot).transform;
            
            pages.Add(rootPage);

            pageCount = 0;

            for (int i = 0; i < EDF_Commands.commands.Count; i++)
            {
                //  skip command if ShowGUI is disabled
                if (!EDF_Commands.commands[i].ShowGUI)
                    continue;

                var currentCmd = EDF_Commands.commands[i].command;

                //  create page hierarchy based on splitting commands by '.'
                if (currentCmd.Contains('.'))
                {
                    var cmdPages = currentCmd.Split('.');
                    var currentPage = ContentRoot;

                    //  generate all the pages
                    Subpages(cmdPages, currentPage, rootPage, pages, EDF_Commands.commands[i]);

                    //  unparent & hide pages so they can be toggled individually
                    foreach (Transform page in pages)
                    {
                        page.SetParent(ContentRoot);
                        page.gameObject.SetActive(false);
                    }

                    //  show root page
                    rootPage.gameObject.SetActive(true);
                    continue;
                }
                //  commands registered to root page
                else { 
                    CreateButton(currentCmd, EDF_Commands.commands[i], rootPage, null); 
                    pageCount++;
                }
                    
            }

            //Debug.Log(rootPage.transform.childCount + " == " + pageCount);

            if (rootPage.transform.childCount != pageCount)
            {
                StartCoroutine(IRuntimeGUI());
                Debug.Log("trying again...");
            }
        }

        //  Create page hierarchy
        void Subpages(string[] cmdPages, Transform currentPage, Transform rootPage, List<Transform> pages, EDF_CommandList.EDF_Command command)
        {
            for (int l = 0; l < cmdPages.Length; l++)
            {
                //  --------------------------
                //  Is command:
                if (l == cmdPages.Length - 1)
                {
                    //  CREATE BUTTON PARENTED TO CURRENTPAGE
                    if (currentPage == ContentRoot)
                        currentPage = rootPage;
                    CreateButton(cmdPages[l], command, currentPage, null);
                    goto end_of_loop;
                }
                //  --------------------------

                //  ==========================
                //  Is page:
                //  Check if page already exists
                foreach (Transform page in currentPage)
                {
                    if (currentPage.name == cmdPages[l])
                        goto end_of_loop; //continue;
                    if (page.name == cmdPages[l]) {
                        currentPage = page;
                        goto end_of_loop; //continue;
                    }
                }
                //  ==========================

                //  --------------------------
                //  create page:
                Transform newPage = Instantiate(folderRoot, currentPage).transform;
                newPage.gameObject.name = cmdPages[l];
                
                //  back button
                var backPage = currentPage.gameObject == ContentRoot.gameObject ? rootPage.gameObject : currentPage.gameObject;
                CreateButton("< Back", EDF_Commands.commands[0], newPage, backPage);  //  assigned command[0] but will never be used
                //  goto page button
                CreateButton(cmdPages[l], command, currentPage == ContentRoot ? rootPage : currentPage, newPage.gameObject);

                currentPage = newPage;
                pages.Add(newPage);
                //  --------------------------

                pageCount++;

            end_of_loop: { }
            }
        }

        EDF_GUIButton CreateButton(string labelText, EDF_CommandList.EDF_Command command, Transform parent, GameObject pageToggle)
        {
            var button = Instantiate(guiButton, parent);
            //  setup button text
            button.label.text = labelText;
            button.name = "(Button)" + labelText;

            //  button opens a new page
            if (pageToggle) {
                button.pageToggle = pageToggle;
                //  do not add pageIcon if it's a back button
                if (labelText == "< Back")
                    return button;
                Instantiate(pageIcon, button.inputRoot);
            }
            //  button executes a command
            else {
                button.command = command.command;
                //  -----------------------
                //  setup argument input
                for(int i = 0; i < command.arguments.Length; i++)
                {
                    //  create input ui
                    var argInputType = command.arguments[i].arg.ToString() == "_Bool" ? boolInput : stringInput;
                    var input = Instantiate(argInputType, button.inputRoot).GetComponent<EDF_GUIInput>();

                    //  setup label text
                    var argType = command.arguments[i].arg.ToString();
                    var argName = command.arguments[i].name == string.Empty ? argType.Substring(1, argType.Length - 1) : command.arguments[i].name;
                    input.label.text = argName;

                    button.inputs.Add(input);

                    //  autoexec OnInput(False) if bool to prevent null references
                    if (argType == "_Bool")
                        input.OnInput("False");
                }
                //  -----------------------
            }

            return button;
        }

        //  hide all pages then re-enable desired one
        public void ShowPage(GameObject newPage)
        {
            foreach (Transform page in pages)
                page.gameObject.SetActive(false);

            newPage.SetActive(true);
        }

        //  clears all pages in content root
        void ClearChildren()
        {
            foreach (Transform child in ContentRoot)
                Destroy(child.gameObject);
        }
    }
}
