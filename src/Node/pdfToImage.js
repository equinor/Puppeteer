const puppeteer = require('puppeteer');
const tmp = require("tmp");
const fs = require('fs');
const fileUrl = require('file-url');
const PDFMerge = require('pdf-merge');
const path = require('path');
const PDFImage = require("pdf-image").PDFImage;

module.exports = function (result, file, page, dpi) {
    //todo
    console.log(file);

    var pdfImage = new PDFImage(file, {
        convertOptions: {
          "-background": "white",
          "-alpha": "remove",
          "-alpha": "off",
          //"-quality": "90",
          "-density": `${dpi}`
        }
      });
    pdfImage.convertPage(page).then(function (imagePath) {
        result(null, imagePath);
        // 0-th page (first page) of the slide.pdf is available as slide-0.png
        //fs.existsSync("/tmp/slide-0.png") // => true
    });

    
};
