<%@ Page Title="" Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true"
    CodeFile="SyncImages.aspx.cs" Inherits="admin_SyncImages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="pnlMain" runat="Server">
    <div class="createform">
        <div class="form-header">
            Synchronize Images to Mobile</div>
        <div class="form-horizontal">
            <div class="alert alert-block">
                Note: Any images under mobile folder will be replaced.</div>
            <div class="alert alert-error" id="mobile-divError" style="display: none;">
                Problem Encountered. Please contact administrator.
                <br />
            </div>
            <div class="control-group" id="mobile-divwait" style="display: none;">
                <div style="text-align: center; width: 100%;">
                    Images synchronization in progress. Please wait...
                    <br />
                    <span id="mobileBatchCompleted">0</span> / <span id="mobileTotalImages">0</span>
                </div>
                <div class="progress progress-striped active" style="margin: 0px auto 0px auto; width: 50%;">
                    <div class="bar" style="width: 100%;">
                    </div>
                </div>
            </div>
        </div>
        <div class="form-footer">
            <input type="button" value="Synchronize Images to Mobile" id="btnSyncmobile" onclick="ProcessSynchronization('mobile');"
                class="btn" />
        </div>
    </div>
    <div class="createform">
        <div class="form-header">
            Synchronize Images to Minicart</div>
        <div class="form-horizontal">
            <div class="alert alert-block">
                Note: Any images under minicart folder will be replaced.</div>
            <div class="alert alert-error" id="minicart-divError" style="display: none;">
                Problem Encountered. Please contact administrator.
                <br />
            </div>
            <div class="control-group" id="minicart-divwait" style="display: none;">
                <div style="text-align: center; width: 100%;">
                    Images synchronization in progress. Please wait....
                    <br />
                    <span id="minicartBatchCompleted">0</span> / <span id="minicartTotalImages">0</span>
                </div>
                <div class="progress progress-striped active" style="margin: 0px auto 0px auto; width: 50%;">
                    <div class="bar" style="width: 100%;">
                    </div>
                </div>
            </div>
        </div>
        <div class="form-footer">
            <input type="button" value="Synchronize Images to Minicart" id="btnSyncminicart" onclick="ProcessSynchronization('minicart');"
                class="btn" />
        </div>
    </div>
    <script type="text/javascript">

        function ProcessSynchronization(imagesFor) {

            var answer = confirm("Are you sure you want to proceed?");

            if (!answer) return;

            $("#btnSyncminicart").hide();
            $("#btnSyncmobile").hide();

            $("#" + imagesFor + "-divwait").show();

            var param = new Object();
            param.TotalImages = 0;
            param.CurrentImageRow = 0;

            StartUpload(param, imagesFor);
        }

        function StartUpload(param, syncType) {

            var paramCopy = param;

            $.ajax({
                type: "POST",
                url: "../ActionService.asmx/SyncImages",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{"totalImages":' + JSON.stringify(paramCopy.TotalImages) +
                      ', "currentImageRow":' + JSON.stringify(paramCopy.CurrentImageRow) +
                      ', "syncType":' + JSON.stringify(syncType) + '}',
                success: function (result) {

                    paramCopy = $.parseJSON(result.d);

                    $("#" + syncType + "BatchCompleted").html(paramCopy.CurrentImageRow);
                    $("#" + syncType + "TotalImages").html(paramCopy.TotalImages);

                    if (paramCopy.CurrentImageRow != paramCopy.TotalImages) {
                        StartUpload(paramCopy, syncType);
                    } else {
                        $("#" + syncType + "-divwait").hide();

                        $("#btnSyncminicart").show();
                        $("#btnSyncmobile").show();

                        $("#" + syncType + "-divError").hide();
                        alert('Images successfully synchronized!');
                    }

                },
                error: function (result, textStatus, errorThrown) {

                    $("#" + syncType + "-divwait").hide();
                    $("#btnSync" + syncType).show();
                    $("#" + syncType + "-divError").show();
                    $("#" + syncType + "-divError").append("<center><h3>" + errorThrown + "</h3>  </center>");
                }
            });

        }

    </script>
</asp:Content>
