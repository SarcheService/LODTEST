class GenericTagsAdmin extends GenericController{
    public searchTree: SearchTreeHelper;
    public nodeSearchID:number;
    private readonly urlGetTags_Tree:string;
    private readonly urlGetTags_List:string;
    private readonly ControllerName:string;
    private readonly btnAddTags:string;
    private readonly PrimaryKey:number;
    private EditModal:ModalHelper;

    constructor(_ControllerName:string,_PrimaryKey:number){
        super();
        this.ControllerName = _ControllerName;
        this.PrimaryKey = _PrimaryKey;
        this.urlGetTags_Tree = "/Admin/Tags/getTreeTags_" + _ControllerName;
        this.urlGetTags_List = `/Admin/Tags/getListTags_${_ControllerName}`;
        this.btnAddTags = "#btnAddTag_" + _ControllerName;
        this.EditModal = new ModalHelper("#modalTagValue");
    }
    public InitHelper(){
        this.initSearchTree();
        this.InitTable();
    }
    private InitTable(){
         this.Events.bind("OnGetPartial",()=>{
            var tableHelper = new TableHelper('#tablaTags_'+ this.ControllerName,true);
        });
        super.getPartial("#divTableTags_" + this.ControllerName, this.urlGetTags_List, { id: this.PrimaryKey });
    }
    private initSearchTree(){
        
        this.searchTree = new SearchTreeHelper("#treeSearchTag_"+this.ControllerName,"#btnExpTag_"+this.ControllerName,"#btnColTag_"+this.ControllerName,"#txtSearchTag_"+this.ControllerName);
        this.searchTree.Events.bind("OnSelectNode", (node)=>{
            
            this.nodeSearchID=node.data.db_id;
            var tipo = node.data.type;
            if (tipo == 'tag') {
                this.enableLink(this.btnAddTags);
                this.setLink(this.btnAddTags,`/Admin/Tags/AddTag_${this.ControllerName}?id=${this.PrimaryKey}&IdTag=${ this.nodeSearchID}`);
            }else {
                this.disableLink(this.btnAddTags);
            }

        });
        this.searchTree.initTree();
        this.searchTree.updateTreeData(this.urlGetTags_Tree,{id:this.PrimaryKey});
    }
    private initTagModal(_TagsParameters:ITagsParameters):void{
        var tagModalForm = new ModalHelper("#modalTagsList");
        
        tagModalForm.initModal();
        tagModalForm.open();
    }
    private InitEditTagModal():void{
        console.log("Entro");
        this.EditModal = new ModalHelper("#modalTagValue");
        $.validator.unobtrusive.parse($("#ediTagForm"));
        this.EditModal.initModal();
        this.EditModal.open();
    }
    private saveTagResult(data:string,_UpdateTreeDataParams:any):void{
        if(data == 'true'){
            this.InitHelper();
            this.alert.toastOk();
            this.EditModal.close();
            this.disableLink(this.btnAddTags);
            return;
        } else {
            this.alert.toastErrorData(data);
        }
    }
}