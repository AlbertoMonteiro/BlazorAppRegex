const myBlazorWorker = new Worker('blazorWebWorker.js', { type: "module" });
window.myBlazorWorker = myBlazorWorker;
myBlazorWorker.addEventListener('message', (e) => {
  const strJson = e.data;
  performance.mark("after");
  performance.measure("Total", "before", "after");
  document.getElementById("elapsed").textContent = `Evaluated in: ${performance.getEntriesByType("measure")[0].duration}ms`;
  const json = strJson && strJson.indexOf('{') !== -1
    ? JSON.stringify((JSON.parse(strJson)), null, 2)
    : strJson;
  window.editor.setValue(json);
  performance.clearMarks();
  performance.clearMeasures();
  performance.clearResourceTimings();
})
document.querySelector("#app").remove();
document.querySelector("#flex-container").style.display = "flex";
const regexElement = document.querySelector("#regex");
const valueElement = document.querySelector("#value");
const substitutionElement = document.querySelector("#substitution");

document.querySelector("#regex-matches").addEventListener("click", () => {
  performance.mark("before");
  window.myBlazorWorker.postMessage(JSON.stringify({ method: "globalMatches", regex: regexElement.value, textValue: valueElement.value }))
});
document.querySelector("#regex-match").addEventListener("click", () => {
  performance.mark("before");
  window.myBlazorWorker.postMessage(JSON.stringify({ method: "oneMatch", regex: regexElement.value, textValue: valueElement.value }))
});
document.querySelector("#regex-substitution").addEventListener("click", () => {
  performance.mark("before");
  window.myBlazorWorker.postMessage(JSON.stringify({ method: "substitution", regex: regexElement.value, textValue: valueElement.value, substitution: substitutionElement.value }))
});
document.querySelector("#regex-list-substitution").addEventListener("click", () => {
  performance.mark("before");
  window.myBlazorWorker.postMessage(JSON.stringify({ method: "listSubstitution", regex: regexElement.value, textValue: valueElement.value, substitution: substitutionElement.value }))
});