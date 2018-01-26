import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import TodoItem from '../shared/todoitem.model';

@Component({
  selector: 'app-listview',
  templateUrl: './listview.component.html',
  styleUrls: ['./listview.component.css']
})
export class ListviewComponent implements OnInit {

  @Input() title: string;
  @Input() items: TodoItem[];
  @Input() panelClass: string;
  @Input() labelClass: string;
  @Output() itemClick = new EventEmitter<TodoItem>();

  constructor() { }

  ngOnInit() {
  }

  onClick(item: TodoItem) {
    this.itemClick.emit(item);
  }

}
