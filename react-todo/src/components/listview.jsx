import * as React from 'react';

const ListView = ({ title, items, panelClass, labelClass, onClick }) => {
  return (
    <div className={"panel " + panelClass}>
      <div className="panel-heading">
        <span>{title}<span className="badge badge-important pull-right">{items.length}</span></span>
      </div>
      <ul className="list-group">
        {items.map(item =>
          <li key={item.id} className="list-group-item">
            <label className={labelClass}>
              <input type="checkbox" onClick={() => onClick(item)} />
              {item.title}
            </label>
            <span className="text-muted pull-right">{item.createTime.toString()}</span>
          </li>
        )}
      </ul>
    </div>
  )
}

export default ListView;
