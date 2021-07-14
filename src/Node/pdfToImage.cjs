module.exports = function(result, file, page, dpi) {
    import puppeteer from "puppeteer";
    import tmp from "tmp";
    import fs from "fs";
    import fileUrl from "file-url";
    import PDFMerge from "pdf-merge";
    import path from "path";
    import PDFImage from "pdf-image".PDFImage;
    //todo
    console.log(file);

    var pdfImage = new PDFImage(file,
        {
            convertOptions: {
                "-background": "white",
                "-alpha": "remove",
                "-alpha": "off",
                //"-quality": "90",
                "-density": `${dpi}`
            }
        });
    pdfImage.convertPage(page).then(function(imagePath) {
        result(null, imagePath);
    });


};