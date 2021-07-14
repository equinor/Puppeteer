module.exports = async function(result, html, config) {
    import puppeteer from "puppeteer";
    import tmp  from "tmp";
    import fs from "fs";
    import fileUrl from "file-url";
    import PDFMerge from "pdf-merge";
    import path from "path";
    const parsedConfig = JSON.parse(config);

    const htmlFileName = await writeFile(html);

    const browser = await createBrowser();
    const pdfPath = await createPdf(browser, htmlFileName, parsedConfig);


    await browser.close();
    await result(null, pdfPath);
};

async function writeFile(html) {
    var tempHtmlFile = tmp.fileSync({
        postfix: ".html"
    });
    +
        await fs.writeFile(tempHtmlFile.name,
            html,
            (err) => {
                if (err) throw err;
                console.log("The file has been saved!");
            });

    return tempHtmlFile.name;
}

async function createBrowser() {
    return await puppeteer.launch({
        args: ["--no-sandbox", "--disable-setuid-sandbox"]
    });
}

function createPdfs(browser, pages) {
    return Promise.all(
        pages.map((filePath) =>
            createPdf(browser, filePath)
        )
    );
}

async function createPdf(browser, fileName, config) {
    const page = await browser.newPage();

    await page.goto(fileUrl(fileName),
        {
            waitUntil: "networkidle2"
        });

    const tmpPdfFileName = tmp.tmpNameSync() + ".pdf";
    await page.pdf({
        path: tmpPdfFileName,
        ...config
    });

    await page.close();

    return tmpPdfFileName;
}