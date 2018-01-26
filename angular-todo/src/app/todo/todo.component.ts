import { Component, OnInit } from '@angular/core';
import TodoItem from '../shared/todoitem.model';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.css']
})
export class TodoComponent implements OnInit {

  todolist: TodoItem[] = [];
  donelist: TodoItem[] = [];
  stamp = 0;
  input = '';

  constructor() { }

  ngOnInit() {
  }

  addTodo() {
    if (this.input) {
      this.todolist.push({
        id: this.stamp,
        title: this.input,
        createTime: new Date()
      });
      this.input = '';
      this.stamp++;
    }
  }

  getDone(task) {
    const index = this.todolist.indexOf(task);
    this.todolist.splice(index, 1);
    this.donelist.push(task);
  }

  discard(task) {
    const index = this.donelist.indexOf(task);
    this.donelist.splice(index, 1);
  }

}
