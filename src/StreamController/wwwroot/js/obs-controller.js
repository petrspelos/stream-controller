/**
 * Get a template from a string
 * https://stackoverflow.com/a/41015840
 * @param  {String} str    The string to interpolate
 * @param  {Object} params The parameters
 * @return {String}        The interpolated string
 */
function interpolate(str, params) {
  let names = Object.keys(params);
  let vals = Object.values(params);
  return new Function(...names, `return \`${str}\`;`)(...vals);
}

const sceneBtnList = document.getElementById('scene-btn-list');
const sourceItemList = document.getElementById('source-item-list');

const sceneBtnTemplate = document.getElementById("t-scene-btn");
const sourceItemTemplate = document.getElementById("t-source-item");

const data = {
  scenes: [],
  activeScene: null,
  sources: []
};

let connection = new signalR.HubConnectionBuilder().withUrl("/obs-hub").build();

connection.on("InputMuteStateChanged", (sourceName, isMuted) => {
  console.log(isMuted ? `MUTED 🔇: ${sourceName}` : `UNMUTED 🔈: ${sourceName}`);
});

connection.on("SourceEnableStateChanged", (itemId, isEnabled) => {
  let source = data.sources.find(s => s.id === itemId);

  if (source) {
    let sourceNameElement = document.querySelector(`[data-source-id="${source.id}"]`).children[0];
    sourceNameElement.classList.remove('text-danger');
    sourceNameElement.classList.remove('text-success');
    sourceNameElement.classList.add(isEnabled ? 'text-success' : 'text-danger');
    console.log(isEnabled ? `SHOWN: ${source.name}` : `HIDDEN: ${source.name}`);
  }
});

connection.on("InputActiveStateChanged", (inputName, isActive) => {
  console.log(`Input ${inputName} active: ${isActive}`);
});

connection.on("ObsConnected", () => {
  console.log("Received: ObsConnected");
  setTimeout(refreshAll, 500);
});

connection.on("ObsDisconnected", () => {
  console.log("Received: ObsDisconnected");
});

connection.on("Scenes", (sceneNames) => {
  console.log("Scene Names:", sceneNames);
  data.scenes = sceneNames;
  sceneBtnList.innerHTML = "";
  let newHtml = "";
  for (let scene of sceneNames) {
    newHtml += interpolate(
      sceneBtnTemplate.innerHTML,
      {name: scene, text: scene}
    );
  }
  sceneBtnList.innerHTML = newHtml;
});

connection.on("ActiveScene", (sceneName) => {
  console.log("Active Scene:", sceneName);
  data.activeScene = sceneName;

  clearButtons();
  document.querySelector(`[data-scene-name="${sceneName}"]`).disabled = true;

  requestRaw("GetSceneSources", sceneName);
});

connection.on("SceneSources", (sources) => {
  console.log('Received Sources:', sources);
  data.sources = sources;
  sourceItemList.innerHTML = "";
  let newHtml = "";
  for (let source of sources) {
    newHtml += interpolate(
      sourceItemTemplate.innerHTML,
      {id: source.id, name: source.name}
    );
  }
  sourceItemList.innerHTML = newHtml;
});

connection.start().then(function () {
  requestData("ObsConnection");
}).catch(function (err) {
  return console.error(err.toString());
});

function setInputEnabled(btn, enabled) {
  let itemId = Number(btn.parentElement.dataset.sourceId);
  requestRaw("SetInputEnabled", data.activeScene, itemId, enabled);
}

function changeSceneClicked(btn) {
  requestRaw("ActivateScene", btn.dataset.sceneName);
}

function addWatchedSource(sourceName) {
  let source = data.sources.find(s => s.name === sourceName);

  if (!source) {
    console.error(`No source item with name '${sourceName}' found in the current scene.`);
    return;
  }

  if (!data.watchedSources.some(s => s.id === source.id)) {
    data.watchedSources.push(source);
    console.log(`Watching ${sourceName} for mute/unmute`);
  }
}

function refreshAll() {
  console.log("Requesting full refresh...");
  requestData("Scenes");
  requestData("ActiveScene");
}

function clearButtons() {
  const elements = document.querySelectorAll('[data-scene-name]');
  for (let element of elements) {
    element.disabled = false;
  }
}

function requestData(type) {
  connection.invoke(`Request${type}`).catch(function (err) {
    return console.error(err.toString());
  });
}

function requestRaw(endpoint, ...data) {
  connection.invoke(endpoint, ...data).catch(function (err) {
    return console.error(err.toString());
  });
}
