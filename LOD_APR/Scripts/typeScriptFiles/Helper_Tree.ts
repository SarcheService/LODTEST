class TreeHelper implements ITreeHelper{
        treeName: string;
        nodeData: any;
        expandBtnName:string;
        collapseBtnName:string;
        searchInputName:string;
        //EVENTOS
        public Events: EventsHelper;
        //OnSelectNode
        //OnGetTreeData
        //OnInitTree

        constructor(_treeName:string){
            this.treeName=_treeName;
            this.Events = new EventsHelper();
            this.expandBtnName='#btnExpand';
            this.collapseBtnName='#btnCollapse';
            this.searchInputName='#treesearch';
        }

        public initTree(): void{

            $(this.treeName).jstree({
                'core': {
                    "check_callback" :  function (op, node, par, pos, more) {
                        //console.debug(par.data.type);
                        // if ((op === "move_node") && node.type == "ro") {
                        //     return false;
                        // }
                        // if ((op === "move_node") && node.type != "ua" && par.data.type!="ua") {
                        //     console.debug("El destino no es UA");
                        //     return false;
                        // }
                        // if ((op === "move_node") && par.id == "#" && more.pos!='i') {
                        //     console.debug("No se puede poner antes del root");
                        //     return false;
                        // }
                        
                        // if((op === "move_node") && more && more.core && !confirm('Are you sure ...')) {
                        //     return false;
                        // }
                        return true;
                    },'data': []},
                'search': {
                    'case_insensitive': false,
                    'show_only_matches' : true
                },
                'plugins': ["search", "sort", "dnd","state"],
            });

             //HACEMOS EL BIND AL COMPONENTE TREEVIEW CUANDO SE SELECCIONE UN NODO
             if(this.Events.on("OnSelectNode")){
                $(this.treeName).on("select_node.jstree", (evt, data)=> {
                    this.nodeData = data.node;
                    this.Events.call("OnSelectNode", data.node);
                    //let hijos = data.node.children.length;
                    //let nodes = data.node.parents;
                });
             }
            //**************************************************************/
            
            let to:any = false;
            $(this.searchInputName).keyup(()=> {
                if (to) { 
                    clearTimeout(to); 
                }
                to = setTimeout(()=> {
                    let v = $(this.searchInputName).val();
                    $(this.treeName).jstree(true).search(v);
                }, 500);
            });
            
            $(this.collapseBtnName).click(()=> {
                $(this.treeName).jstree('close_all');
            });

            $(this.expandBtnName).click(()=> {
                $(this.treeName).jstree('open_all');
            });

            this.Events.call("OnInitTree");

        }

        public updateTreeData(_urlGetTree:string,_params:any={}){
            $.ajax({
                url: _urlGetTree,
                data: _params,
                type: 'GET',
                success: (result)=> {
                    let lstNodes = JSON.parse(result);
                    $(this.treeName).jstree(true).settings.core.data = lstNodes;
                    $(this.treeName).jstree(true).refresh();
                    this.Events.call("OnGetTreeData");
                },
                error: function (xhr, status, error) {
                    var alert:AlertHelper = new AlertHelper();
                    alert.toastErrorData(xhr.responseText);
                }
            });
        }

}

class TreeHelperAdmin implements ITreeHelper{
    treeName: string;
    nodeData: any;
    expandBtnName:string;
    collapseBtnName:string;
    searchInputName:string;
    CutNode:any;
    SelectedNode:any;
    public Events: EventsHelper;
 
    constructor(_treeName:string){
        this.treeName=_treeName;
        this.Events = new EventsHelper();
        this.expandBtnName='#btnExpand';
        this.collapseBtnName='#btnCollapse';
        this.searchInputName='#treesearch';
    }

    public initTree(): void{

        $(this.treeName).jstree({
            'core': {
                "check_callback" :  function (op, node, par, pos, more) {
                    return true;
                },'data': []},
            'search': {
                'case_insensitive': false,
                'show_only_matches' : true
            },
            'plugins': ["search", "sort","state", "contextmenu", "wholerow"],
            "contextmenu":{         
                "items": ($node)=> {
                    return {
                        "Cut": {
                            "separator_before": false,
                            "separator_after": false,
                            "label": "Cortar",
                            "_disabled": this.DisableCutContext(),
                            "action": (obj)=> { 
                                this.CutNode = $node;
                                $(this.treeName).jstree().cut($node);
                            }
                        },                         
                        "Paste": {
                            "separator_before": false,
                            "separator_after": false,
                            "label": this.GetPasteContextName(),
                            "_disabled": this.DisablePasteContext(),
                            "action": (obj)=> { 
                                $(this.treeName).jstree().paste($node);
                            }
                        }
                    };
                }
            }
        });

         //HACEMOS EL BIND AL COMPONENTE TREEVIEW CUANDO SE SELECCIONE UN NODO
         if(this.Events.on("OnSelectNode")){
            $(this.treeName).on("select_node.jstree", (evt, data)=> {
                this.nodeData = data.node;
                this.Events.call("OnSelectNode", data.node);
            });
         }
        //**************************************************************/
        
        let to:any = false;
        $(this.searchInputName).keyup(()=> {
            if (to) { 
                clearTimeout(to); 
            }
            to = setTimeout(()=> {
                let v = $(this.searchInputName).val();
                $(this.treeName).jstree(true).search(v);
            }, 500);
        });
        
        $(this.collapseBtnName).click(()=> {
            $(this.treeName).jstree('close_all');
        });

        $(this.expandBtnName).click(()=> {
            $(this.treeName).jstree('open_all');
        });

        this.Events.call("OnInitTree");

    }

    public GetCutNode(){
        return this.CutNode;
    }

    public DisableCutContext(){
        
        var tipoDest = this.SelectedNode.data.type;

        if(tipoDest=="ro"){
            return true;
        }else{
            return false;
        }
    }

    public DisablePasteContext(){
        
        if(!$(this.treeName).jstree().can_paste()){
            return true;
        }

        var tipo = this.CutNode.data.type;
        var tipoDest = this.SelectedNode.data.type;
        var result = false;
                     
        if(tipo=="proy"){
           if(tipoDest=="proy" || tipoDest=="obra" || tipoDest=="con" || tipoDest=="bit"){
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
           }
        }
        else if(tipo=="obra"){                 
            if(tipoDest=="obra" || tipoDest=="con" || tipoDest=="bit"){
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
           }
        }
        else if(tipo=="carp"){
            if(tipoDest=="con" || tipoDest=="bit"){
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
           }
        } 
        else if(tipo=="con"){
           if(tipoDest=="con" || tipoDest=="bit"){
            result = true;
            console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
           }
        } 
        else if(tipo=="bit"){
            if(tipoDest=="con" || tipoDest=="bit"){
                result = true;
                console.log("Un " + tipo + " No se puede pegar en: " + tipoDest);
           }
        }

        return result;

    }

    public GetPasteContextName(){
                   
        if(!$(this.treeName).jstree().can_paste()){
            return "Pegar Aquí";
        }

        var tipo = this.CutNode.data.type;
        var tipoDest = this.SelectedNode.data.type;
        var result = "Pegar " + this.CutNode.text + " Aquí";
                                 
        if(tipo=="proy"){
           if(tipoDest=="proy" || tipoDest=="obra" || tipoDest=="con" || tipoDest=="bit"){
                result = "Este Proyecto No se puede pegar aquí";
           }
        }
        else if(tipo=="obra"){                 
            if(tipoDest=="obra" || tipoDest=="con" || tipoDest=="bit"){
                result = "La Obra " + this.CutNode.text + " No se puede pegar aquí";
           }
        }
        else if(tipo=="carp"){
            if(tipoDest=="con" || tipoDest=="bit"){
                result = "La Carpeta " + this.CutNode.text + " No se puede pegar aquí";
           }
        } 
        else if(tipo=="con"){
           if(tipoDest=="con" || tipoDest=="bit"){
            result = "El Contrato " + this.CutNode.text + " No se puede pegar aquí";
           }
        } 
        else if(tipo=="bit"){
            if(tipoDest=="con" || tipoDest=="bit"){
                result = "La Bitácora " + this.CutNode.text + " No se puede pegar aquí";
           }
        }

        return result;

    }

    public updateTreeData(_urlGetTree:string,_params:any={}){
        $.ajax({
            url: _urlGetTree,
            data: _params,
            type: 'GET',
            success: (result)=> {
                let lstNodes = JSON.parse(result);
                $(this.treeName).jstree(true).settings.core.data = lstNodes;
                $(this.treeName).jstree(true).refresh();
                this.Events.call("OnGetTreeData");
            },
            error: function (xhr, status, error) {
                var alert:AlertHelper = new AlertHelper();
                alert.toastErrorData(xhr.responseText);
            }
        });
    }

}


class SearchTreeHelper implements ITreeHelper{
        treeName: string;
        nodeData: any;
        expandBtnName:string;
        collapseBtnName:string;
        searchInputName:string;
        //EVENTOS
         public Events: EventsHelper;
        //OnSelectNode
        //OnGetTreeData
        //OnInitTree

        constructor(_treeName:string,_expandBtnName:string,_collapseBtnName:string,_searchInputName:string){
            this.Events = new EventsHelper();
            this.treeName=_treeName;
            this.expandBtnName=_expandBtnName;
            this.collapseBtnName=_collapseBtnName;
            this.searchInputName=_searchInputName;
            //this.initTree();
        }

        public initTree(): void{
            
            $(this.treeName).jstree({
                'search': {
                    'case_insensitive': false,
                    'show_only_matches' : true
                },
                'plugins': ["search", "sort","state"],
            });

             //HACEMOS EL BIND AL COMPONENTE TREEVIEW CUANDO SE SELECCIONE UN NODO
             if(this.Events.on("OnSelectNode")){
                $(this.treeName).on("select_node.jstree", (evt, data)=> {
                    this.nodeData = data.node;
                    this.Events.call("OnSelectNode", data.node);
                    //let hijos = data.node.children.length;
                    //let nodes = data.node.parents;
                });
             }
            //**************************************************************/
            
            let to:any = false;
            $(this.searchInputName).keyup(()=> {
                if (to) { 
                    clearTimeout(to); 
                }
                to = setTimeout(()=> {
                    let v = $(this.searchInputName).val();
                    $(this.treeName).jstree(true).search(v);
                }, 500);
            });
            
            $(this.collapseBtnName).click(()=> {
                $(this.treeName).jstree('close_all');
            });

            $(this.expandBtnName).click(()=> {
                $(this.treeName).jstree('open_all');
            });

        }

        public updateTreeData(_urlGetTree:string,_data:any){
            $.ajax({
                url: _urlGetTree,
                data: _data,
                type: 'GET',
                success: (result)=> {
                    let lstNodes = JSON.parse(result);
                    $(this.treeName).jstree(true).settings.core.data = lstNodes;
                    $(this.treeName).jstree(true).refresh();
                    this.Events.call("OnGetTreeData");
                },
                error: function (xhr, status, error) {
                    var alert:AlertHelper = new AlertHelper();
                    alert.toastErrorData(xhr.responseText);
                }
            });
        }
    
}