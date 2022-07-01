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

    async connect() {
        const response = await fetch('connect', {
            method: "POST",
        });
        const data = await response.json();
        this.state = { data: data, loading: true };
    }

    async setMark(x, y) {
        const response = await fetch('setMark?sessionId=0&x=' + x + '&y=' + y, {
            method: "POST",
        });
        const data = await response.json();
        this.state = { data: data, loading: true };
        this.render();
    }

  renderForecastsTable(data) {
    return (
      <table className='playground' aria-labelledby="tabelLabel">
            <tbody>
                {data.Field.Cells.map((row) => 
                    <tr key={row[0].x}>{row.map((cell) =>
                        <td key={cell.x + "" + cell.y} onClick={() => this.setMark(cell.x, cell.y)}> {cell.Value}</td>)}
                    </tr>      
            )}
            </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
        ? 
      <div>
        <h1 id="tabelLabel">Wait pls</h1>
            <p><em>Loading...</em></p>
      </div>
        :
        <div>
            <h1 id="tabelLabel" onClick={() => this.setMark(1, 1)}>{this.state.data.Player1.Name + "-" + this.state.data.Player1.Mark}</h1>
            <button id="connect" onClick={ () => this.connect()}>Connect</button>
            {this.renderForecastsTable(this.state.data)}
        </div>;

    return (contents);
  }

  async populateWeatherData() {
      const response = await fetch('session?sessionId=0');
      const data = await response.json();
      console.log(data);
    this.setState({ data: data, loading: false });
    }
}
