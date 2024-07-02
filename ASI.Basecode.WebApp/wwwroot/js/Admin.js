const adminBody = document.querySelector(".admin-body");
const agentsContainer = document.querySelector(".agents-container");
const editDetails = document.querySelector('[data-admin-edit-details]');
const agentsList = document.querySelectorAll(".agents")
const modalContainer = document.querySelector("[data-admin-modal-container]")

const adminCloseBtn = document.querySelector("[data-admin-edit-close-btn]")
const adminCancelBtn = document.querySelector("[data-admin-edit-cancel-btn]")
const adminSaveBtn = document.querySelector("[data-admin-edit-save-btn]")

const adminEditBtn = document.querySelector("[data-admin-edit-btn]")
const adminEditForm = document.querySelector("[data-modal-edit-form]")

adminEditForm.addEventListener("submit", (e) => {
    e.preventDefault();
})

adminEditBtn.addEventListener("click", () => {
    console.log("clicking open btn", modalContainer, adminEditBtn)
    modalContainer.classList.remove("hidden")
    adminEditForm.classList.remove("hidden")
})


adminCancelBtn.addEventListener("click", () => {
    console.log("clicking cancel btn", modalContainer)
    modalContainer.classList.add("hidden")
    adminEditForm.classList.add("hidden")
})

adminCloseBtn.addEventListener("click", () => {
    console.log("clicking close btn", modalContainer)
    modalContainer.classList.add("hidden")
    adminEditForm.classList.add("hidden")
})

adminSaveBtn.addEventListener("click", () => {
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