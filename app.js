const STORAGE_KEY = "focusflow_tasks_v1";
const taskForm = document.getElementById("task-form");
const taskInput = document.getElementById("task-input");
const taskList = document.getElementById("task-list");

let tasks = JSON.parse(localStorage.getItem(STORAGE_KEY) || "[]");

function saveTasks() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(tasks));
}

function renderTasks() {
  taskList.innerHTML = "";
  if (tasks.length === 0) {
    const empty = document.createElement("li");
    empty.textContent = "暂无任务，先添加一个吧！";
    empty.style.color = "#9ca3af";
    taskList.appendChild(empty);
    return;
  }

  tasks.forEach((task) => {
    const li = document.createElement("li");
    li.className = `task-item ${task.done ? "done" : ""}`;

    const text = document.createElement("span");
    text.textContent = task.title;

    const actions = document.createElement("div");
    actions.className = "task-buttons";

    const toggleBtn = document.createElement("button");
    toggleBtn.textContent = task.done ? "撤销" : "完成";
    toggleBtn.addEventListener("click", () => {
      task.done = !task.done;
      saveTasks();
      renderTasks();
    });

    const delBtn = document.createElement("button");
    delBtn.textContent = "删除";
    delBtn.className = "delete-btn";
    delBtn.addEventListener("click", () => {
      tasks = tasks.filter((t) => t.id !== task.id);
      saveTasks();
      renderTasks();
    });

    actions.append(toggleBtn, delBtn);
    li.append(text, actions);
    taskList.appendChild(li);
  });
}

taskForm.addEventListener("submit", (event) => {
  event.preventDefault();
  const title = taskInput.value.trim();
  if (!title) return;
  tasks.unshift({ id: crypto.randomUUID(), title, done: false });
  taskInput.value = "";
  saveTasks();
  renderTasks();
});

const WORK_SECONDS = 25 * 60;
let remaining = WORK_SECONDS;
let timerId = null;

const timeDisplay = document.getElementById("time-display");
const startBtn = document.getElementById("start-btn");
const pauseBtn = document.getElementById("pause-btn");
const resetBtn = document.getElementById("reset-btn");

function formatTime(totalSeconds) {
  const minutes = Math.floor(totalSeconds / 60)
    .toString()
    .padStart(2, "0");
  const seconds = (totalSeconds % 60).toString().padStart(2, "0");
  return `${minutes}:${seconds}`;
}

function renderTimer() {
  timeDisplay.textContent = formatTime(remaining);
}

startBtn.addEventListener("click", () => {
  if (timerId) return;
  timerId = setInterval(() => {
    remaining -= 1;
    if (remaining <= 0) {
      remaining = 0;
      clearInterval(timerId);
      timerId = null;
      alert("一个番茄钟完成！休息一下吧 ☕");
    }
    renderTimer();
  }, 1000);
});

pauseBtn.addEventListener("click", () => {
  if (!timerId) return;
  clearInterval(timerId);
  timerId = null;
});

resetBtn.addEventListener("click", () => {
  clearInterval(timerId);
  timerId = null;
  remaining = WORK_SECONDS;
  renderTimer();
});

renderTasks();
renderTimer();
