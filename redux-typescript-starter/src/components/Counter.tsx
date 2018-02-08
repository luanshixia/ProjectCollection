import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { ApplicationState } from '../store';
import * as CounterStore from '../store/Counter';
import * as WeatherForecasts from '../store/WeatherForecasts';

// https://github.com/DefinitelyTyped/DefinitelyTyped/issues/9951
// Another workaround is to use any.
const { connect } = require('react-redux');

type CounterProps =
  CounterStore.CounterState
  & typeof CounterStore.actionCreators
  & RouteComponentProps<{}>;

// Wire up the React component to the Redux store
@connect(
  (state: ApplicationState) => state.counter, // Selects which state properties are merged into the component's props
  CounterStore.actionCreators                 // Selects which action creators are merged into the component's props
)
export default class Counter extends React.Component<CounterProps, {}> {
  public render() {
    return (
    <div>
      <h1>Counter</h1>

      <p>This is a simple example of a React component.</p>

      <p>Current count: <strong>{this.props.count}</strong></p>

      <button onClick={() => { this.props.increment(); }}>Increment</button>
    </div>
    );
  }
}
