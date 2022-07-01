import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { data: Object, loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(data) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
            <tbody>
                {data.Field.Cells.map((row) => 
                        <tr key={row[0].x}>{row.map((cell) =>
                            <td key={cell.x + "" + cell.y}> {cell.Value} </td>)}
                        </tr>      
            )}
            </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.data);

    return (
      <div>
        <h1 id="tabelLabel" >Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async populateWeatherData() {
      const response = await fetch('session?sessionId=0');
      const data = await response.json();
      console.log(data);
    this.setState({ data: data, loading: false });
  }
}
