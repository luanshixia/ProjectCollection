import React, { Component } from 'react';
import Todo from './components/todo';
import logo from './logo.svg';
import './App.css';

class App extends Component {
  render() {
    return (
      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <h1 className="App-title">Welcome to React</h1>
        </header>
        <p className="App-intro">
          To get started, edit <code>src/App.js</code> and save to reload.
        </p>
        <div className="container">
          <div className="starter-template">
            <h1>
              Start right here...
            </h1>
            <p className="lead">
              Type in one task below.
            </p>
            <Todo />
          </div>
        </div>
      </div>
    );
  }
}

export default App;
