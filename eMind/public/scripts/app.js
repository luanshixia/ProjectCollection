
const root = Node.fromSpecString('{"content":"Root","children":[{"content":"Children 1","children":[{"content":"Children 4"},{"content":"Children 5"}]},{"content":"Children 2"},{"content":"Children 3"}]}');

root.updateAllRecursively();

// eslint-disable-next-line no-console
root.walk(node => console.log(node.toDebugString()));

const map = new MindMap(root);

document.getElementById('svg-container').innerHTML = map.toSvgString();
