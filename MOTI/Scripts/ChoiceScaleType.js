$(document).ready(function () {
    var selectCType = $("#CType").change(function () {
        var selectScaleType = $("#ScaleType");
        if (selectCType[0].selectedIndex == 1) {
            selectScaleType[0][1].disabled = false;
            selectScaleType[0][2].disabled = false;
            selectScaleType[0][3].disabled = true;
            selectScaleType[0][4].disabled = true;
            selectScaleType[0][5].disabled = true;
            selectScaleType[0][6].disabled = true;
            if (selectScaleType[0].selectedIndex > 2)
            {
                selectScaleType[0].selectedIndex = 0;
            }
        }
        else if (selectCType[0].selectedIndex == 2) {
            selectScaleType[0][1].disabled = true;
            selectScaleType[0][2].disabled = true;
            selectScaleType[0][3].disabled = false;
            selectScaleType[0][4].disabled = false;
            selectScaleType[0][5].disabled = false;
            selectScaleType[0][6].disabled = false;
            if (selectScaleType[0].selectedIndex <= 2) {
                selectScaleType[0].selectedIndex = 0;
            }
        }
    });
    var selectScaleType = $("#ScaleType");
    if (selectCType[0].selectedIndex == 1) {
        selectScaleType[0][1].disabled = false;
        selectScaleType[0][2].disabled = false;
        selectScaleType[0][3].disabled = true;
        selectScaleType[0][4].disabled = true;
        selectScaleType[0][5].disabled = true;
        selectScaleType[0][6].disabled = true;
        if (selectScaleType[0].selectedIndex > 2) {
            selectScaleType[0].selectedIndex = 0;
        }
    }
    else if (selectCType[0].selectedIndex == 2) {
        selectScaleType[0][1].disabled = true;
        selectScaleType[0][2].disabled = true;
        selectScaleType[0][3].disabled = false;
        selectScaleType[0][4].disabled = false;
        selectScaleType[0][5].disabled = false;
        selectScaleType[0][6].disabled = false;
        if (selectScaleType[0].selectedIndex <= 2) {
            selectScaleType[0].selectedIndex = 0;
        }
    }
});