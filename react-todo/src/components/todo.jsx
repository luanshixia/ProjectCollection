import * as React from 'react';
import ListView from './listview';

class Todo extends React.Component {
  constructor(props) {
    super(props);
    this.tempData = {
      stamp: 0
    };
    this.state = {
      todolist: [],
      donelist: [],
      input: ''
    }
  }

  render() {
    const { todolist, donelist, input } = this.state;

    const handleChange = event => {
      this.setState({
        ...this.state,
        input: event.target.value
      });
    };

    const addTodo = () => {
      if (this.state.input) {
        this.setState({
          ...this.state,
          todolist: [
            ...this.state.todolist,
            {
              id: this.tempData.stamp,
              title: this.state.input,
              createTime: new Date()
            }
          ],
          input: ''
        });

        this.tempData.stamp++;
      }
    };

    const getDone = task => {
      this.setState({
        ...this.state,
        todolist: this.state.todolist.filter(item => item !== task),
        donelist: [
          ...this.state.donelist,
          task
        ]
      });
    };

    const discard = task => {
      this.setState({
        ...this.state,
        donelist: this.state.donelist.filter(item => item !== task)
      });
    };

    return (
      <div id="todo-area">
        <div className="panel input-group">
          <input className="form-control" type="text" value={input} onChange={handleChange} onKeyPress={event => event.key === 'Enter' && addTodo()} />
          <span className="input-group-btn">
            <a className="btn btn-default" onClick={addTodo}>Add</a>
          </span>
        </div>
        <ListView title="Todo List" items={todolist} panelClass="panel-primary" onClick={getDone} />
        <ListView title="Done List" items={donelist} panelClass="panel-success" labelClass="text-crossline" onClick={discard} />
      </div>
    )
  }
}

export default Todo;
