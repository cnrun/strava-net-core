import React, { Component } from 'react';

export class Strava extends Component {
  displayName = Strava.name

  constructor(props) {
    super(props);
    this.state = { activities: [], loading: true };

    fetch('api/SampleData/GetActivities')
      .then(response => response.json())
      .then(data => {
        this.setState({ activities: data, loading: false });
      });
  }

  static renderActivitiesTable(activities) {
    return (
      <table className='table'>
        <thead>
          <tr>
            <th>Type</th>
            <th>Name</th>
          </tr>
        </thead>
        <tbody>
          {activities.map(activity =>
            <tr>
              <td>{activity.type}</td>
              <td>{activity.name}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Strava.renderActivitiesTable(this.state.activities);

    return (
      <div>
        <h1>Strava activities</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }
}
