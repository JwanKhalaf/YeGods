/// <binding Clean='clean' />
"use strict";

const { src, dest, parallel, series } = require("gulp");
const rimraf = require("rimraf");
const concat = require("gulp-concat");
const cssmin = require("gulp-cssmin");
const uglify = require("gulp-uglify");
const sass = require("gulp-sass");
const rename = require("gulp-rename");

let paths = {
  webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.sass = paths.webroot + "scss/**/*.scss";
paths.css = paths.webroot + "css/**/*.css";
paths.adminCss = paths.webroot + "css/admin.css";
paths.clientCss = paths.webroot + "css/client.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.adminCssDest = paths.webroot + "css/admin.min.css";
paths.clientCssDest = paths.webroot + "css/client.min.css";

// clear tasks

function clearJavascriptFiles(cb) {
  rimraf(paths.concatJsDest, cb);
}

function clearCssFiles(cb) {
  rimraf(paths.css, cb);
}

function clearLibraries(cb) {
  rimraf("./wwwroot/lib", cb);
}

// process tasks

function convertSassToCss() {
  return (
    src(paths.sass)
      .pipe(sass.sync().on('error', sass.logError))
      .pipe(dest("./wwwroot/css/"))
  );
}

// minify tasks

function minifyJavascript(cb) {
  src([paths.js, "!" + paths.minJs], { base: "." })
    .pipe(uglify())
    .pipe(rename({ suffix: '.min' }))
    .pipe(dest("."));

  cb();
}

function minifyCss(cb) {
  src([paths.adminCss, "!" + paths.minCss])
    .pipe(concat(paths.adminCssDest))
    .pipe(cssmin())
    .pipe(dest("."));

  src([paths.clientCss, "!" + paths.minCss])
    .pipe(concat(paths.clientCssDest))
    .pipe(cssmin())
    .pipe(dest("."));

  cb();
}

// copy tasks

function copyjQuery() {
  return src(["./node_modules/jquery/**/*"])
    .pipe(dest("./wwwroot/lib/jquery"));
}

function copyjQueryValidation() {
  return src(["./node_modules/jquery-validation/**/*"])
    .pipe(dest("./wwwroot/lib/jquery-validation"));
}

function copyjQueryValidationUnobtrusive() {
  return src(["./node_modules/jquery-validation-unobtrusive/**/*"])
    .pipe(dest("./wwwroot/lib/jquery-validation-unobtrusive"));
}

function copyBootstrap() {
  return src(["./node_modules/bootstrap/**/*"])
    .pipe(dest("./wwwroot/lib/bootstrap"));
}

function copyJsAutocomplete() {
  return src(["./node_modules/js-autocomplete/**/*"])
    .pipe(dest("./wwwroot/lib/js-autocomplete"));
}

function copySimpleMde() {
  return src(["./node_modules/simplemde/**/*"])
    .pipe(dest("./wwwroot/lib/simplemde"));
}

exports.default = series(parallel(clearJavascriptFiles, clearCssFiles, clearLibraries), convertSassToCss, parallel(copyjQuery, copyjQueryValidation, copyjQueryValidationUnobtrusive, copyBootstrap, copyJsAutocomplete, copySimpleMde), parallel(minifyJavascript, minifyCss));
