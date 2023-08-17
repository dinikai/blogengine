let anchor = window.location.hash.substring(1);

function activateMenuItem(id) {
    document.getElementById(id).classList.add("side-bar-item-active");
}

function removeItem(name) {
    return confirm(`Are you sure to remove ${name}?`);
}

function filterByClass(className, containerId) {
    for (let element of document.getElementById(containerId).children) {
        if (element.classList.contains(className))
            element.classList.remove("filtered");
        else
            element.classList.add("filtered");
    };
}

function filter(element, className, containerId) {
    filterByClass(className, containerId);
    for (let child of element.parentElement.children) {
        child.classList.remove("filter-selected");
    }
    element.classList.add("filter-selected");
}

function openTab(tabButton, tabId, containerId) {
    for (let element of document.getElementById(containerId).children) {
        if (element.id == tabId)
            element.classList.remove("filtered");
        else
            element.classList.add("filtered");
    };

    for (let child of tabButton.parentElement.children) {
        child.classList.remove("tab-button-selected");
    }
    tabButton.classList.add("tab-button-selected");
}