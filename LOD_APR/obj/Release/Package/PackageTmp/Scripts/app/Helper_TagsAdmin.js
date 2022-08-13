var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var GenericTagsAdmin = (function (_super) {
    __extends(GenericTagsAdmin, _super);
    function GenericTagsAdmin(_ControllerName, _PrimaryKey) {
        var _this = _super.call(this) || this;
        _this.ControllerName = _ControllerName;
        _this.PrimaryKey = _PrimaryKey;
        _this.urlGetTags_Tree = "/Admin/Tags/getTreeTags_" + _ControllerName;
        _this.urlGetTags_List = "/Admin/Tags/getListTags_" + _ControllerName;
        _this.btnAddTags = "#btnAddTag_" + _ControllerName;
        _this.EditModal = new ModalHelper("#modalTagValue");
        return _this;
    }
    GenericTagsAdmin.prototype.InitHelper = function () {
        this.initSearchTree();
        this.InitTable();
    };
    GenericTagsAdmin.prototype.InitTable = function () {
        var _this = this;
        this.Events.bind("OnGetPartial", function () {
            var tableHelper = new TableHelper('#tablaTags_' + _this.ControllerName, true);
        });
        _super.prototype.getPartial.call(this, "#divTableTags_" + this.ControllerName, this.urlGetTags_List, { id: this.PrimaryKey });
    };
    GenericTagsAdmin.prototype.initSearchTree = function () {
        var _this = this;
        this.searchTree = new SearchTreeHelper("#treeSearchTag_" + this.ControllerName, "#btnExpTag_" + this.ControllerName, "#btnColTag_" + this.ControllerName, "#txtSearchTag_" + this.ControllerName);
        this.searchTree.Events.bind("OnSelectNode", function (node) {
            _this.nodeSearchID = node.data.db_id;
            var tipo = node.data.type;
            if (tipo == 'tag') {
                _this.enableLink(_this.btnAddTags);
                _this.setLink(_this.btnAddTags, "/Admin/Tags/AddTag_" + _this.ControllerName + "?id=" + _this.PrimaryKey + "&IdTag=" + _this.nodeSearchID);
            }
            else {
                _this.disableLink(_this.btnAddTags);
            }
        });
        this.searchTree.initTree();
        this.searchTree.updateTreeData(this.urlGetTags_Tree, { id: this.PrimaryKey });
    };
    GenericTagsAdmin.prototype.initTagModal = function (_TagsParameters) {
        var tagModalForm = new ModalHelper("#modalTagsList");
        tagModalForm.initModal();
        tagModalForm.open();
    };
    GenericTagsAdmin.prototype.InitEditTagModal = function () {
        console.log("Entro");
        this.EditModal = new ModalHelper("#modalTagValue");
        $.validator.unobtrusive.parse($("#ediTagForm"));
        this.EditModal.initModal();
        this.EditModal.open();
    };
    GenericTagsAdmin.prototype.saveTagResult = function (data, _UpdateTreeDataParams) {
        if (data == 'true') {
            this.InitHelper();
            this.alert.toastOk();
            this.EditModal.close();
            this.disableLink(this.btnAddTags);
            return;
        }
        else {
            this.alert.toastErrorData(data);
        }
    };
    return GenericTagsAdmin;
}(GenericController));
