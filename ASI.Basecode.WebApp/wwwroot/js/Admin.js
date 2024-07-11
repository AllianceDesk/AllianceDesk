const adminBody = document.querySelector(".admin-body");
const agentsContainer = document.querySelector(".agents-container");
const editDetails = document.querySelector('[data-admin-edit-details]');
const agentsList = document.querySelectorAll("[data-viewuser-agent]")
const modalContainer = document.querySelector("[data-admin-modal-container]")
const deleteModalContainer = document.querySelector("[data-admin-delete-modal-container]")

const navItems = document.querySelectorAll("[data-nav-item]")


/* BTNS */

const adminEditCloseBtn = document.querySelector("[data-admin-edit-close-btn]")
const adminEditCancelBtn = document.querySelector("[data-admin-edit-cancel-btn]")
const adminEditSaveBtn = document.querySelector("[data-admin-edit-save-btn]")

const adminAddCloseBtn = document.querySelector("[data-admin-add-close-btn]")
const adminAddCancelBtn = document.querySelector("[data-admin-add-cancel-btn]")
const adminAddSaveBtn = document.querySelector("[data-admin-add-save-btn]")

const adminDelCloseBtn = document.querySelector("[data-admin-delete-close-btn]")
const adminDelCancelBtn = document.querySelector("[data-admin-delete-cancel-btn]")
const adminDelSaveBtn = document.querySelector("[data-admin-delete-save-btn]")

const adminEditBtn = document.querySelector("[data-admin-edit-btn]")
const adminEditForm = document.querySelector("[data-modal-edit-form]")

const adminAddBtn = document.querySelector("[data-admin-add-btn]")
const adminAddForm = document.querySelector("[data-modal-add-form]")

const adminDelBtn = document.querySelector("[data-admin-delete-btn]")
const adminDelForm = document.querySelector("[data-admin-delete-modal]")

/* DELETE */

adminDelBtn.addEventListener("click", () => {
    console.log("clicking open btn", modalContainer, adminEditBtn)
    deleteModalContainer.classList.remove("hidden")
    adminDelForm.classList.remove("hidden")
})


adminDelCancelBtn.addEventListener("click", () => {
    console.log("clicking cancel btn", modalContainer)
    deleteModalContainer.classList.add("hidden")
    adminDelForm.classList.add("hidden")
})

adminDelCloseBtn.addEventListener("click", () => {
    console.log("clicking close btn", modalContainer)
    deleteModalContainer.classList.add("hidden")
    adminDelForm.classList.add("hidden")
})

adminDelSaveBtn.addEventListener("click", () => {
    console.log("clicking save btn", modalContainer)
    deleteModalContainer.classList.add("hidden")
    adminDelForm.classList.add("hidden")
})

/* ADD */

adminAddForm.addEventListener("submit", (e) => {
    e.preventDefault();
})

adminAddBtn.addEventListener("click", () => {
    console.log("clicking open btn", modalContainer, adminEditBtn)
    modalContainer.classList.remove("hidden")
    adminAddForm.classList.remove("hidden")
})


adminAddCancelBtn.addEventListener("click", () => {
    console.log("clicking cancel btn", modalContainer)
    modalContainer.classList.add("hidden")
    adminAddForm.classList.add("hidden")
})

adminAddCloseBtn.addEventListener("click", () => {
    console.log("clicking close btn", modalContainer)
    modalContainer.classList.add("hidden")
    adminAddForm.classList.add("hidden")
})

adminAddSaveBtn.addEventListener("click", () => {
    console.log("clicking save btn", modalContainer)
    modalContainer.classList.add("hidden")
    adminAddForm.classList.add("hidden")
})

/* EDIT */
adminEditForm.addEventListener("submit", (e) => {
    e.preventDefault();
})

adminEditBtn.addEventListener("click", () => {
    console.log("clicking open btn", modalContainer, adminEditBtn)
    modalContainer.classList.remove("hidden")
    adminEditForm.classList.remove("hidden")
})


adminEditCancelBtn.addEventListener("click", () => {
    console.log("clicking cancel btn", modalContainer)
    modalContainer.classList.add("hidden")
    adminEditForm.classList.add("hidden")
})

adminEditCloseBtn.addEventListener("click", () => {
    console.log("clicking close btn", modalContainer)
    modalContainer.classList.add("hidden")
    adminEditForm.classList.add("hidden")
})

adminEditSaveBtn.addEventListener("click", () => {
    console.log("clicking save btn", modalContainer)
    modalContainer.classList.add("hidden")
    adminEditForm.classList.add("hidden")
})

agentsList.forEach(agent => {
    agent.addEventListener("click", () => {
        if (agent.classList.contains("selected-agent")) {
            adminBody.classList.remove("edit-active");
            agent.classList.remove("selected-agent");
            editDetails.classList.add("hidden");
            return;
        }
        agentsList.forEach(a => a.classList.remove("selected-agent"));
        agent.classList.add("selected-agent");
        adminBody.classList.add("edit-active");
        editDetails.classList.remove("hidden");
    })
})


navItems.forEach(navItem => {
    navItem.addEventListener("click", () => {
        navItems.forEach(a => a.classList.remove("active"));
        navItem.classList.add("active");
    })
})